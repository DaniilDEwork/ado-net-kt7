let resources = [];
let editingId = null;
let lastPeriod = null;
const types = { room: 'Номер', table: 'Стол', hall: 'Зал' };
const form = document.getElementById('booking-form');
const resourceInput = document.getElementById('resource');
const customerInput = document.getElementById('customer');
const startInput = document.getElementById('start');
const endInput = document.getElementById('end');
const availableStart = document.getElementById('available-start');
const availableEnd = document.getElementById('available-end');
const typeInput = document.getElementById('resource-type');

function localDate(value) {
    const date = new Date(value);
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}T${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`;
}

function displayDate(value) {
    return new Date(value).toLocaleString('ru-RU', { dateStyle: 'short', timeStyle: 'short' });
}

function resetBooking() {
    editingId = null;
    form.reset();
    document.getElementById('form-title').textContent = 'Новое бронирование';
    document.getElementById('save-booking').textContent = 'Забронировать';
    const start = new Date();
    start.setDate(start.getDate() + 1);
    start.setHours(12, 0, 0, 0);
    startInput.value = localDate(start);
    endInput.value = localDate(new Date(start.getTime() + 3600000));
}

function period(start, end) {
    if (!start || !end || new Date(start) >= new Date(end)) throw new Error('Окончание должно быть позже начала');
    return { start: new Date(start).toISOString(), end: new Date(end).toISOString() };
}

async function loadAvailable(query, showResponse = true) {
    const params = new URLSearchParams(query);
    const available = await apiRequest('GET', `/resources/available?${params}`, undefined, showResponse);
    const tbody = document.getElementById('available-body');
    tbody.replaceChildren();
    for (const resource of available) {
        const row = document.createElement('tr');
        cell(row, resource.id);
        cell(row, resource.name);
        cell(row, types[resource.type] || resource.type);
        tbody.append(row);
    }
    if (!available.length) emptyRow(tbody, 3, 'Свободных ресурсов нет');
    lastPeriod = query;
    document.getElementById('available-period').textContent = `${displayDate(query.start)} - ${displayDate(query.end)}, ${types[query.type] || 'Все типы'}`;
}

async function loadBookings(showResponse = false) {
    const bookings = await apiRequest('GET', '/bookings', undefined, showResponse);
    const tbody = document.getElementById('bookings-body');
    tbody.replaceChildren();
    for (const booking of bookings) {
        const resource = resources.find(item => item.id === booking.resourceId);
        const row = document.createElement('tr');
        cell(row, booking.id);
        cell(row, resource?.name || booking.resourceId);
        cell(row, booking.customerName);
        cell(row, displayDate(booking.start));
        cell(row, displayDate(booking.end));
        const actions = cell(row, '');
        actionButton(actions, 'Изменить', async () => {
            const current = await apiRequest('GET', `/bookings/${booking.id}`);
            editingId = current.id;
            resourceInput.value = current.resourceId;
            customerInput.value = current.customerName;
            startInput.value = localDate(current.start);
            endInput.value = localDate(current.end);
            document.getElementById('form-title').textContent = `Изменение бронирования с ID ${current.id}`;
            document.getElementById('save-booking').textContent = 'Сохранить изменения';
        });
        actionButton(actions, 'Отменить', async () => {
            await apiRequest('DELETE', `/bookings/${booking.id}`);
            if (editingId === booking.id) resetBooking();
            await loadBookings();
            if (lastPeriod) await loadAvailable(lastPeriod, false);
        });
        tbody.append(row);
    }
    if (!bookings.length) emptyRow(tbody, 6, 'Бронирований нет');
}

form.addEventListener('submit', event => {
    event.preventDefault();
    perform(async () => {
        const range = period(startInput.value, endInput.value);
        await apiRequest(editingId === null ? 'POST' : 'PUT', editingId === null ? '/bookings' : `/bookings/${editingId}`, {
            resourceId: Number(resourceInput.value), customerName: customerInput.value, ...range
        });
        await loadBookings();
        if (lastPeriod) await loadAvailable(lastPeriod, false);
    });
});
document.getElementById('available-form').addEventListener('submit', event => {
    event.preventDefault();
    perform(async () => {
        const query = period(availableStart.value, availableEnd.value);
        if (typeInput.value) query.type = typeInput.value;
        await loadAvailable(query);
    });
});
document.getElementById('reset-booking').addEventListener('click', resetBooking);
document.getElementById('refresh-bookings').addEventListener('click', () => perform(() => loadBookings(true)));
resetBooking();
availableStart.value = startInput.value;
availableEnd.value = endInput.value;
perform(async () => {
    resources = await apiRequest('GET', '/resources', undefined, false);
    for (const resource of resources) resourceInput.add(new Option(`${resource.name} (${types[resource.type] || resource.type})`, resource.id));
    await loadBookings(true);
}).then(() => {
    document.getElementById('save-booking').disabled = resources.length === 0;
});
