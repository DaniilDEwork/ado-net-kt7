let editingId = null;
const form = document.getElementById('student-form');
const nameInput = document.getElementById('name');
const ageInput = document.getElementById('age');

function resetStudent() {
    editingId = null;
    form.reset();
    document.getElementById('form-title').textContent = 'Добавление студента';
    document.getElementById('save-student').textContent = 'Добавить студента';
}

async function loadStudents(showResponse = false) {
    const students = await apiRequest('GET', '/students', undefined, showResponse);
    const tbody = document.getElementById('students-body');
    tbody.replaceChildren();
    for (const student of students) {
        const row = document.createElement('tr');
        cell(row, student.id);
        cell(row, student.name);
        cell(row, student.age);
        const actions = cell(row, '');
        actionButton(actions, 'Просмотр', async () => {
            await apiRequest('GET', `/students/${student.id}`);
        });
        actionButton(actions, 'Изменить', async () => {
            const current = await apiRequest('GET', `/students/${student.id}`);
            editingId = current.id;
            nameInput.value = current.name;
            ageInput.value = current.age;
            document.getElementById('form-title').textContent = `Изменение студента с ID ${current.id}`;
            document.getElementById('save-student').textContent = 'Сохранить изменения';
        });
        actionButton(actions, 'Удалить', async () => {
            await apiRequest('DELETE', `/students/${student.id}`);
            if (editingId === student.id) resetStudent();
            await loadStudents();
        });
        tbody.append(row);
    }
    if (!students.length) emptyRow(tbody, 4, 'Список студентов пуст');
}

form.addEventListener('submit', event => {
    event.preventDefault();
    perform(async () => {
        const body = { name: nameInput.value, age: Number(ageInput.value) };
        await apiRequest(editingId === null ? 'POST' : 'PUT', editingId === null ? '/students' : `/students/${editingId}`, body);
        await loadStudents();
    });
});
document.getElementById('reset-student').addEventListener('click', resetStudent);
document.getElementById('refresh-students').addEventListener('click', () => perform(() => loadStudents(true)));
perform(() => loadStudents(true));
