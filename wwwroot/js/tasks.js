const statuses = { new: 'Новая', 'in-progress': 'В работе', completed: 'Выполнена' };
const filter = document.getElementById('status-filter');

async function loadTasks(showResponse = false) {
    const url = filter.value ? `/tasks?status=${encodeURIComponent(filter.value)}` : '/tasks';
    const tasks = await apiRequest('GET', url, undefined, showResponse);
    const tbody = document.getElementById('tasks-body');
    tbody.replaceChildren();
    for (const task of tasks) {
        const row = document.createElement('tr');
        cell(row, task.id);
        cell(row, task.title);
        cell(row, task.description);
        const statusCell = cell(row, '');
        const select = document.createElement('select');
        for (const [value, title] of Object.entries(statuses)) select.add(new Option(title, value));
        select.value = task.status;
        statusCell.append(select);
        const actions = cell(row, '');
        actionButton(actions, 'Сохранить статус', async () => {
            await apiRequest('PUT', `/tasks/${task.id}/status`, { status: select.value });
            await loadTasks();
        });
        actionButton(actions, 'Удалить', async () => {
            await apiRequest('DELETE', `/tasks/${task.id}`);
            await loadTasks();
        });
        tbody.append(row);
    }
    if (!tasks.length) emptyRow(tbody, 5, 'Задачи не найдены');
}

document.getElementById('task-form').addEventListener('submit', event => {
    event.preventDefault();
    perform(async () => {
        await apiRequest('POST', '/tasks', {
            title: document.getElementById('title').value,
            description: document.getElementById('description').value
        });
        filter.value = '';
        await loadTasks();
    });
});
filter.addEventListener('change', () => perform(() => loadTasks(true)));
document.getElementById('refresh-tasks').addEventListener('click', () => perform(() => loadTasks(true)));
perform(() => loadTasks(true));
