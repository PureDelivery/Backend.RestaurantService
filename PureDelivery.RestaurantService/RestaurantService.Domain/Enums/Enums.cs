using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantService.Domain.Enums
{
    public enum OptionType
    {
        Single,     // Один выбор (размер пиццы)
        Multiple    // Множественный выбор (добавки)
    }

    public enum MenuCategory
    {
        Appetizers = 0,     // Закуски
        Soups = 1,          // Супы  
        MainCourse = 2,     // Основные блюда
        Pizza = 3,          // Пицца
        Pasta = 4,          // Паста
        Salads = 5,         // Салаты
        Desserts = 6,       // Десерты
        Beverages = 7,      // Напитки
        AlcoholicDrinks = 8,// Алкоголь
        Sides = 9,          // Гарниры
        Seafood = 10,       // Морепродукты
        Meat = 11,          // Мясные блюда
        Vegetarian = 12,    // Вегетарианские
        Breakfast = 13,     // Завтраки
        Lunch = 14,         // Обеды
        Dinner = 15         // Ужины
    }

    [Flags]
    public enum Allergen
    {
        None = 0,
        Gluten = 1,          // Глютен
        Dairy = 2,           // Молочные продукты  
        Eggs = 4,            // Яйца
        Fish = 8,            // Рыба
        Shellfish = 16,      // Моллюски
        TreeNuts = 32,       // Орехи
        Peanuts = 64,        // Арахис
        Soy = 128,           // Соя
        Sesame = 256,        // Кунжут
        Sulfites = 512,      // Сульфиты
        Celery = 1024,       // Сельдерей
        Mustard = 2048,      // Горчица
        Lupin = 4096         // Люпин
    }

    [Flags]
    public enum DietaryTag
    {
        None = 0,
        Vegetarian = 1,      // Вегетарианское
        Vegan = 2,           // Веганское
        GlutenFree = 4,      // Без глютена
        DairyFree = 8,       // Без молочных продуктов
        Organic = 16,        // Органическое
        LowCarb = 32,        // Низкоуглеводное
        LowFat = 64,         // Низкожирное
        HighProtein = 128,   // Высокобелковое
        Keto = 256,          // Кето-диета
        Paleo = 512,         // Палео-диета
        Halal = 1024,        // Халяль
        Kosher = 2048,       // Кошерное
        Spicy = 4096,        // Острое
        LowSodium = 8192,    // Низкосоленое
        SugarFree = 16384    // Без сахара
    }

    [Flags]
    public enum CuisineType
    {
        None = 0,
        Italian = 1,         // Итальянская
        Chinese = 2,         // Китайская
        Japanese = 4,        // Японская
        Mexican = 8,         // Мексиканская
        Indian = 16,         // Индийская
        French = 32,         // Французская
        Thai = 64,           // Тайская
        Greek = 128,         // Греческая
        American = 256,      // Американская
        Turkish = 512,       // Турецкая
        Korean = 1024,       // Корейская
        Vietnamese = 2048,   // Вьетнамская
        Georgian = 4096,     // Грузинская
        Russian = 8192,      // Русская
        Ukrainian = 16384,   // Украинская
        Lebanese = 32768,    // Ливанская
        Spanish = 65536,     // Испанская
        German = 131072,     // Немецкая
        British = 262144,    // Британская
        Mediterranean = 524288 // Средиземноморская
    }

    [Flags]
    public enum RestaurantTag
    {
        None = 0,
        FastDelivery = 1,        // Быстрая доставка
        HealthyFood = 2,         // Здоровая еда
        LuxuryDining = 4,        // Премиум заведение
        FamilyFriendly = 8,      // Семейное место
        Romantic = 16,           // Романтическая обстановка
        CasualDining = 32,       // Повседневная еда
        BusinessLunch = 64,      // Бизнес-ланч
        LateNight = 128,         // Работает допоздна
        Breakfast = 256,         // Завтраки
        Buffet = 512,            // Шведский стол
        Takeaway = 1024,         // На вынос
        Catering = 2048,         // Кейтеринг
        Organic = 4096,          // Органические продукты
        LocalIngredients = 8192, // Местные продукты
        ChefSpecial = 16384,     // Авторская кухня
        Budget = 32768,          // Бюджетно
        Premium = 65536,         // Дорого
        NewOpening = 131072,     // Новое заведение
        PopularChoice = 262144   // Популярный выбор
    }
}
