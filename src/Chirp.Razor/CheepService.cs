// using Microsoft.Data.Sqlite;
// using System.Data;
// namespace Chirp.Razor;
//
// public record CheepViewModel(string Author, string Message, string Timestamp);
//
// public interface ICheepService
// {
//     public List<CheepViewModel> GetCheeps(int page);
//     public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
// }
//
// public class CheepService : ICheepService
// {
//     private const int CheepsPerPage = 32;
//     // These would normally be loaded from a database for example
//     private static readonly DBFacade _cheepsDB = new();
//
//     public List<CheepViewModel> GetCheeps(int page)
//     {
//         page = page <= 0 ? 1 : page;
//         return _cheepsDB.GetCheeps(CheepsPerPage * (page - 1), CheepsPerPage);
//     }
//
//     public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
//     {
//        page = page <= 0 ? 1 : page; 
//        return _cheepsDB.GetCheepsFromAuthor(author, CheepsPerPage * (page - 1), CheepsPerPage);
//     }
//
// }
