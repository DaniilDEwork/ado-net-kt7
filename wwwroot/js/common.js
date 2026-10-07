async function apiRequest(method, url, body, showResponse = true) {
    const options = { method };
    if (body !== undefined) {
        options.headers = { 'Content-Type': 'application/json' };
        options.body = JSON.stringify(body);
    }
    const response = await fetch(url, options);
    const text = await response.text();
    let data = null;
    if (text) {
        try { data = JSON.parse(text); } catch { data = text; }
    }
    if (showResponse) {
        document.getElementById('response-url').textContent = `${method} ${url}`;
        document.getElementById('response-status').textContent = `${response.status} ${response.statusText}`;
        document.getElementById('response-body').textContent = text
            ? (typeof data === 'string' ? data : JSON.stringify(data, null, 2))
            : 'Запись удалена, тело ответа отсутствует';
    }
    if (!response.ok) {
        const message = data?.message || Object.values(data?.errors || {}).flat().join(', ') || 'Не удалось выполнить запрос';
        throw new Error(message);
    }
    return data;
}

let busy = false;
async function perform(action) {
    if (busy) return;
    busy = true;
    const buttons = [...document.querySelectorAll('button')];
    const disabled = buttons.map(button => button.disabled);
    buttons.forEach(button => button.disabled = true);
    document.getElementById('error').textContent = '';
    try {
        await action();
    } catch (error) {
        document.getElementById('error').textContent = error.message;
    } finally {
        buttons.forEach((button, index) => button.disabled = disabled[index]);
        busy = false;
    }
}

function cell(row, value) {
    const td = document.createElement('td');
    td.textContent = value;
    row.append(td);
    return td;
}

function actionButton(parent, title, action) {
    const button = document.createElement('button');
    button.type = 'button';
    button.textContent = title;
    button.addEventListener('click', () => perform(action));
    parent.append(button);
}

function emptyRow(tbody, columns, text) {
    const row = document.createElement('tr');
    cell(row, text).colSpan = columns;
    tbody.append(row);
}
