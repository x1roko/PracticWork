using TaskManagerMiac.Models;

namespace TaskManagerMiac.Data
{
    /// <summary>
    /// Статический класс в котором можно прописать стартовые данные для таблиц в бд
    /// </summary>
    public static class InitialDbData
    {
        public static List<Group> Groups { get; set; } = new List<Group>
        {
            new Group
            {
                IdGroup = 1,
                Title = "Отдел Информационных Технологий И Защиты Информации"
            },
            new Group
            {
                IdGroup = 2,
                Title = "Не определён"
            },
            new Group
            {
                IdGroup = 3,
                Title = "Финансово-Экономическая Служба"
            },
            new Group
            {
                IdGroup = 4,
                Title = "Отдел Медицинской Статистики И Мониторинга"
            },
            new Group
            {
                IdGroup = 5,
                Title = "Отдел Информатизации Здравоохранения И Лекарственного Обеспечения"
            },
            new Group
            {
                IdGroup = 6,
                Title = "Директор"
            },
            new Group
            {
                IdGroup = 7,
                Title = "Отдел Материально Технического Снабжения"
            },
            new Group
            {
                IdGroup = 8,
                Title = "Отдел Реализации Национальных Проектов"
            },
            new Group
            {
                IdGroup = 9,
                Title = "Отдел Консультирования И Аналитики В Сфере Закупок"
            },
            new Group
            {
                IdGroup = 10,
                Title = "Региональный Ситуационный Центр По Вопросам Здравоохранения"
            },
            new Group
            {
                IdGroup = 11,
                Title = "Дистанционный Консультативный Центр Лучевой Диагностики"
            },
        };

        public static List<Priority> Priorities { get; set; } = new List<Priority>
        {
            new Priority
            {
                IdPriority = 1,
                Title = "Низкий",
                Weight = 0
            },
            new Priority
            {
                IdPriority = 2,
                Title = "Средний",
                Weight = 50
            },
            new Priority
            {
                IdPriority = 3,
                Title = "Высокий",
                Weight = 100
            },
            new Priority
            {
                IdPriority = 4,
                Title = "Максимальный",
                Weight = 1000
            },
        };
        public static List<Role> Roles { get; set; } = new List<Role>
        {
            new Role
            {
                IdRole = 1,
                Title = "root"
            },
            new Role
            {
                IdRole = 2,
                Title = "admin"
            },
            new Role
            {
                IdRole = 3,
                Title = "group_admin"
            },
            new Role
            {
                IdRole = 4,
                Title = "default"
            },
        };

        public static List<User> Users { get; set; } = new List<User>
        {
            new User
            {
                Firstname = "Администратор",
                IdGroup = 1,
                IdRole = 1,
                IsActive = true,
                Password = BCrypt.Net.BCrypt.HashPassword( "12345"),
                Snils = "admin@a.ru",
                Surname = " ",
                IdUser = 1
            }
        };

        public static List<TaskState> TaskStates { get; set; } = new List<TaskState>
        {
            new TaskState
            {
                IdTaskState = 1,
                Title = "Создана"
            },
            new TaskState
            {
                IdTaskState = 2,
                Title = "В работе"
            },
            new TaskState
            {
                IdTaskState = 3,
                Title = "Одобрена"
            },
            new TaskState
            {
                IdTaskState = 4,
                Title = "Отклонена"
            },
            new TaskState
            {
                IdTaskState = 5,
                Title = "Ожидает отклонения руководителя"
            },
            new TaskState
            {
                IdTaskState = 6,
                Title = "Ожидает одобрения руководителя"
            },
        };

        public static List<TaskType> TaskTypes { get; set; } = new List<TaskType>
        {
            new TaskType
            {
                IdTaskType = 1,
                Title = "Финансовая"
            },
            new TaskType
            {
                IdTaskType = 2,
                Title = "Обычная"
            },
            new TaskType
            {
                IdTaskType = 4,
                Title = "Персональная"
            }
        };

        /// <summary>
        /// Пути заявок 
        /// Используются для автоматической пересылки заявки по отделам
        /// Например, мы хотим чтобы заявка с типом "Финансовая" 
        /// сначала отправлялясь в отдел "Отдел Материально Технического Снабжения",
        /// потом в "Финансово-Экономическая Служба",
        /// потом к директору 
        /// после того как директор одобрит заявку (нажмет "Выполнить"), 
        /// она будет завершена и получит статус "одобрена"
        /// </summary>
        public static List<TaskTypePath> TaskTypePaths { get; set; } = new List<TaskTypePath>
        {
            new TaskTypePath
            {
                IdTaskType = 1,
                IdGroup = 7, //"Отдел Материально Технического Снабжения"
                Index = 1, //номер отдела по пути заявки
            },
            new TaskTypePath
            {
                IdTaskType = 1,
                IdGroup = 3, //"Финансово-Экономическая Служба"
                Index = 2, //номер отдела по пути заявки
            },
            new TaskTypePath
            {
                IdTaskType = 1,
                IdGroup = 6, //"Директор"
                Index = 3, //номер отдела по пути заявки
            },
        };
    }
}
