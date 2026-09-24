using System;
using System.Collections.Generic;

namespace LibrarySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Book> books = new List<Book>
            {
                new Book("978-0-13-468599-1", "Clean Code",
                    new[] { "Robert C. Martin" }, new DateTime(2008, 8, 1), 34.99m),
                new Book("978-1-4919-1889-0", "Fluent Python",
                    new[] { "Luciano Ramalho" }, new DateTime(2015, 8, 20), 49.99m),
                new Book("978-0-596-00712-6", "Head First Design Patterns",
                    new[] { "Eric Freeman", "Elisabeth Robson" }, new DateTime(2004, 10, 25), 44.99m)
            };

            // a) User-defined delegate
            Console.WriteLine("--- a) User-defined delegate: BookFunctionDelegate ---");
            BookFunctionDelegate getTitle = BookFunctions.GetTitle;
            LibraryEngine.ProcessBooks(books, getTitle);

            // b) Proper built-in delegate: Func<Book, string>
            Console.WriteLine("\n--- b) Built-in delegate: Func<Book, string> ---");
            Func<Book, string> getAuthors = BookFunctions.GetAuthors;
            LibraryEngine.ProcessBooks(books, getAuthors);

            // c) Anonymous method (GetISBN)
            Console.WriteLine("\n--- c) Anonymous method (GetISBN) ---");
            BookFunctionDelegate getIsbn = delegate (Book B)
            {
                return B.ISBN;
            };
            LibraryEngine.ProcessBooks(books, getIsbn);

            // d) Lambda expression (GetPublicationDate)
            Console.WriteLine("\n--- d) Lambda expression (GetPublicationDate) ---");
            Func<Book, string> getPublicationDate = B => B.PublicationDate.ToString("yyyy-MM-dd");
            LibraryEngine.ProcessBooks(books, getPublicationDate);

            Console.WriteLine();
        }
    }
}
