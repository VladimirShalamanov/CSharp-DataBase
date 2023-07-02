namespace BookShop;

using BookShop.Models.Enums;
using System.Globalization;
using Data;
using Initializer;
using System.Text;

public class StartUp
{
    public static void Main()
    {
        using var context = new BookShopContext();
        //DbInitializer.ResetDatabase(context);

        //var res = GetMostRecentBooks(context);

        //var input = Console.ReadLine();

        //var res = GetBooksByAuthor(context, input!);

        //var res = CountBooks(context, int.Parse(input!));

        //Console.WriteLine(res);

        //IncreasePrices(context);
        //Console.WriteLine("Successful increase prices!");

        int numberOfRemoveBooks = RemoveBooks(context);
        Console.WriteLine(numberOfRemoveBooks);
    }

    // 02
    public static string GetBooksByAgeRestriction(BookShopContext context, string command)
    {
        try
        {
            AgeRestriction age = Enum.Parse<AgeRestriction>(command, true);

            var bookTitles = context.Books
                .Where(b => b.AgeRestriction == age)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    // 03
    public static string GetGoldenBooks(BookShopContext context)
    {
        var bookTitles = context.Books
            .Where(b => b.EditionType == EditionType.Gold &&
                        b.Copies < 5000)
            .OrderBy(b => b.BookId)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    // 04
    public static string GetBooksByPrice(BookShopContext context)
    {
        var bookInfo = context.Books
            .Where(b => b.Price > 40)
            .OrderByDescending(b => b.Price)
            .Select(b => $"{b.Title} - ${b.Price:f2}")
            .ToArray();

        return string.Join(Environment.NewLine, bookInfo);
    }

    // 05
    public static string GetBooksNotReleasedIn(BookShopContext context, int year)
    {
        var bookTitles = context.Books
            .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
            .OrderBy(b => b.BookId)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    // 06
    public static string GetBooksByCategory(BookShopContext context, string input)
    {
        var categories = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();

        var bookTitles = context.Books
            .Where(b => b.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
            .OrderBy(b => b.Title)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    // 07
    public static string GetBooksReleasedBefore(BookShopContext context, string date)
    {
        var bookInfo = context.Books
            .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
            .OrderByDescending(b => b.ReleaseDate)
            .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}")
            .ToArray();

        return string.Join(Environment.NewLine, bookInfo);
    }

    // 08
    public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
    {
        var fullNamesAuthors = context.Authors
            .Where(a => a.FirstName.EndsWith(input))
            .OrderBy(a => a.FirstName)
            .ThenBy(a => a.LastName)
            .Select(f => $"{f.FirstName} {f.LastName}")
            .ToArray();

        return string.Join(Environment.NewLine, fullNamesAuthors);
    }

    // 09
    public static string GetBookTitlesContaining(BookShopContext context, string input)
    {
        var titlesBooks = context.Books
            .Where(b => b.Title.ToLower().Contains(input.ToLower()))
            .OrderBy(b => b.Title)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, titlesBooks);
    }

    // 10
    public static string GetBooksByAuthor(BookShopContext context, string input)
    {
        var titlesAndAuthors = context.Books
            .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
            .OrderBy(b => b.BookId)
            .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
            .ToArray();

        return string.Join(Environment.NewLine, titlesAndAuthors);
    }

    // 11
    public static int CountBooks(BookShopContext context, int lengthCheck)
    {
        return context.Books
            .Where(b => b.Title.Length > lengthCheck)
            .Select(b => b.BookId)
            .ToArray()
            .Count();
    }

    // 12
    public static string CountCopiesByAuthor(BookShopContext context)
    {
        var authorsWithBooks = context.Authors
                        .OrderByDescending(a => a.Books.Sum(b => b.Copies))
                        .Select(a => $"{a.FirstName} {a.LastName} - {a.Books.Sum(b => b.Copies)}")
                        .ToArray();

        return string.Join(Environment.NewLine, authorsWithBooks);
    }

    // 13
    public static string GetTotalProfitByCategory(BookShopContext context)
    {
        var books = context.Categories
            .Select(c => new
            {
                Category = c.Name,
                Profit = c.CategoryBooks.Sum(b => b.Book.Price * b.Book.Copies)
            })
            .OrderByDescending(b => b.Profit)
            .ThenBy(c => c.Category)
            .Select(b => $"{b.Category} ${b.Profit:f2}")
            .ToArray();

        return string.Join(Environment.NewLine, books);
    }

    // 14
    public static string GetMostRecentBooks(BookShopContext context)
    {
        var catBooks = context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                CategoryName = c.Name,
                Subtitles = c.CategoryBooks
                            .OrderByDescending(b => b.Book.ReleaseDate)
                            .Take(3)
                            .Select(b => new
                            {
                                BTitle = b.Book.Title,
                                BReleaseDate = b.Book.ReleaseDate!.Value.Year
                            })
                            .ToArray()
            })
            .ToArray();

        var sb = new StringBuilder();

        foreach (var c in catBooks)
        {
            sb.AppendLine($"--{c.CategoryName}");

            foreach (var b in c.Subtitles)
            {
                sb.AppendLine($"{b.BTitle} ({b.BReleaseDate})");
            }
        }

        return sb.ToString().TrimEnd();
    }

    // 15
    public static void IncreasePrices(BookShopContext context)
    {
        var books = context.Books
            .Where(b => b.ReleaseDate!.Value.Year < 2010)
            .ToArray();

        foreach (var b in books)
        {
            b.Price += 5;
        }

        context.SaveChanges();
    }

    // 16
    public static int RemoveBooks(BookShopContext context)
    {
        var books = context.Books
            .Where(b => b.Copies < 4200)
            .ToArray();

        context.RemoveRange(books);
        context.SaveChanges();

        return books.Length;
    }
}