﻿// js для слабаков ("1" + 1 = 11)
// ладно я наврал

function deleteRow(Id, type) {
    // Я же js гений, зачем мне 2 функции писать, вот с 1 веселее

    var url = "";
    if (type == "class") {
        url = '/Classes/Delete/' + Id
    } else {
        url = '/Character/Delete/' + Id
    }



    $.ajax({
        url: url,
        type: 'POST',
        success: function (response) {
            $('#class-row-' + Id).remove();
        },
        error: function (xhr, status, error) {
            alert('Произошла ошибка при удалении.');
        }
    });
}


$(document).ready(function () {
    $('#saveCharacterClassBtn').on('click', function () {
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').text('');

        var formData = {
            Name: $('#className').val(),
            Description: $('#classDescription').val(),
            BaseDamage: $('#baseDamage').val(),
            BaseHealth: $('#baseHealth').val()
        };

        $.ajax({
            url: '/Classes/Create',
            type: 'POST',
            data: JSON.stringify(formData),
            contentType: 'application/json',
            success: function (response) {
                if (response.success) {
                    $('#addCharacterClassModal').modal('hide');

                    var newRow = `
                        <tr id="class-row-${response.id}">
                            <th scope="row">${response.id}</th>
                            <td>${formData.Name}</td>
                            <td>${formData.Description}</td>
                            <td>${formData.BaseDamage}</td>
                            <td>${formData.BaseHealth}</td>
                            <td>
                                <a href="#" class="btn btn-primary">Изменить</a>
                                <button class="btn btn-danger" onclick="deleteRow(${response.id}, 'class')">Удалить</button>
                            </td>
                        </tr>
                    `;

                    $('table tbody').append(newRow);

                    $('#characterForm')[0].reset();
                } else {
                    if (response.errors) {
                        response.errors.forEach(function (error) {
                            if (error.includes('Название')) {
                                $('#className').addClass('is-invalid');
                                $('#classNameError').text(error);
                            }
                            if (error.includes('Описание')) {
                                $('#classDescription').addClass('is-invalid');
                                $('#classDescriptionError').text(error);
                            }
                            if (error.includes('Базовый урон')) {
                                $('#baseDamage').addClass('is-invalid');
                                $('#baseDamageError').text(error);
                            }
                            if (error.includes('Базовые ХП')) {
                                $('#baseHealth').addClass('is-invalid');
                                $('#baseHealthError').text(error);
                            }
                        });
                    } else if (response.error) {
                        alert(response.error);
                    }
                }
            },
            error: function () {
                alert('Произошла ошибка при отправке данных');
            }
        });
    });



    // Добавление персонажа по факту оба метода идентичны.

    $('#saveCharacterBtn').on('click', function () {
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').text('');

        var formData = {
            Name: $('#characterName').val(),
            Level: parseInt($('#characterLevel').val()),
            Experience: parseInt($('#characterExperience').val()),
            ClassId: $('#characterClass').val()
        };

        $.ajax({
            url: '/Character/Create',
            type: 'POST',
            data: JSON.stringify(formData),
            contentType: 'application/json',
            success: function (response) {
                if (response.success) {
                    $('#addCharacterModal').modal('hide');

                    var newRow = `
                        <tr id="class-row-${response.id}">
                            <th scope="row">${response.id}</th>
                            <td>${formData.Name}</td>
                            <td>${formData.Level}</td>
                            <td>${formData.Experience}</td>
                            <td>${$('#characterClass option:selected').text()}</td>
                            <td>
                                <a href="#" class="btn btn-primary">Изменить</a>
                                <button class="btn btn-danger" onclick="deleteRow(${response.id}, 'character')">Удалить</button>
                            </td>
                        </tr>
                    `;

                    $('table tbody').append(newRow);
                } else {
                    if (response.errors) {
                        response.errors.forEach(function (error) {
                            if (error.includes('Имя')) {
                                $('#characterName').addClass('is-invalid');
                                $('#characterNameError').text(error);
                            }
                            if (error.includes('Уровень')) {
                                $('#characterLevel').addClass('is-invalid');
                                $('#characterLevelError').text(error);
                            }
                            if (error.includes('Опыт')) {
                                $('#characterExperience').addClass('is-invalid');
                                $('#characterExperienceError').text(error);
                            }
                            if (error.includes('Класс')) {
                                $('#characterClass').addClass('is-invalid');
                                $('#characterClassError').text(error);
                            }
                        });
                    } else if (response.error) {
                        alert(response.error);
                    }
                }
            },
            error: function () {
                alert('Произошла ошибка при отправке данных');
            }
        });
    });


    $(document).on('click', '.edit-class-btn', function () {
        const id = $(this).data('id');
        const name = $(this).data('name');
        const description = $(this).data('description');
        const baseDamage = $(this).data('basedamage');
        const baseHealth = $(this).data('basehealth');

        $('#editClassId').val(id);
        $('#editClassName').val(name);
        $('#editClassDescription').val(description);
        $('#editBaseDamage').val(baseDamage);
        $('#editBaseHealth').val(baseHealth);

        $('#editCharacterClassModal').modal('show');
    });

    $('#updateCharacterClassBtn').on('click', function () {
        const characterClass = {
            Id: $('#editClassId').val(),
            Name: $('#editClassName').val(),
            Description: $('#editClassDescription').val(),
            BaseDamage: $('#editBaseDamage').val(),
            BaseHealth: $('#editBaseHealth').val()
        };

        $.ajax({
            url: '/Classes/Edit',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(characterClass),
            success: function (response) {
                if (response.success) {
                    const row = $(`#class-row-${characterClass.Id}`);
                    row.find('td:nth-child(2)').text(characterClass.Name);
                    row.find('td:nth-child(3)').text(characterClass.Description);
                    row.find('td:nth-child(4)').text(characterClass.BaseDamage);
                    row.find('td:nth-child(5)').text(characterClass.BaseHealth);

                    $('#editCharacterClassModal').modal('hide');
                } else {
                    alert('Ошибка: ' + (response.error || 'неизвестная ошибка'));
                }
            },
            error: function () {
                alert('Произошла ошибка при редактировании класса персонажа');
            }
        });
    });

    $(document).on('click', '.edit-character-btn', function () {
        const id = $(this).data('id');
        const name = $(this).data('name');
        const level = $(this).data('level');
        const experience = $(this).data('experience');
        const class_id = $(this).data('class-id');

        $('#editCharacterId').val(id);
        $('#EditCharacterName').val(name);
        $('#EditCharacterLevel').val(level);
        $('#EditCharacterExperience').val(experience);
        $('#EditCharacterClass').val(class_id);

        $('#EditCharacterModal').modal('show');
    });

    $('#updateCharacterBtn').on('click', function () {
        const character = {
            Id: $('#editCharacterId').val(),
            Name: $('#EditCharacterName').val(),
            Level: $('#EditCharacterLevel').val(),
            Experience: $('#EditCharacterExperience').val(),
            ClassId: $('#EditCharacterClass').val()
        };

        console.log(character);

        $.ajax({
            url: '/Character/Edit',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(character),
            success: function (response) {
                if (response.success) {
                    const row = $(`#class-row-${character.Id}`);
                    row.find('td:nth-child(2)').text(character.Name);
                    row.find('td:nth-child(3)').text(character.Level);
                    row.find('td:nth-child(4)').text(character.Experience);

                    // доделаю потом, надо сделать получение названия класса по id
                    //row.find('td:nth-child(5)').text(character.ClassId);

                    $('#EditCharacterModal').modal('hide');
                } else {
                    alert('Ошибка: ' + (response.error || 'неизвестная ошибка'));
                }
            },
            error: function () {
                alert('Произошла ошибка при редактировании класса персонажа');
            }
        });
    });
});