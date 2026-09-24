using System;
using System.Collections.Generic;

namespace LibrarySystem
{
    // a) User-defined delegate with the same signature as the BookFunctions methods
    public delegate string BookFunctionDelegate(Book B);

    public class LibraryEngine
    {
        // Overload #1 - accepts the user-defined delegate (case a)
        public static void ProcessBooks(List<Book> bList, /*Pointer To BookFunciton*/ BookFunctionDelegate fPtr)
        {
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }

        // Overload #2 - accepts the built-in Func<> delegate (case b),
        // and is what makes the anonymous method (c) and lambda (d) cases possible too,
        // since both have the exact same shape: Book -> string.
        public static void ProcessBooks(List<Book> bList, /*Pointer To BookFunciton*/ Func<Book, string> fPtr)
        {
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }
    }
}
