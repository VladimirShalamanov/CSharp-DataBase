namespace MusicHub;

using System;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Data;
using Initializer;
using MusicHub.Data.Models;

public class StartUp
{
    public static void Main()
    {
        MusicHubDbContext context =
            new MusicHubDbContext();

        DbInitializer.ResetDatabase(context);

        //Test your solutions here

        //string res = ExportAlbumsInfo(context, 9);
        string res = ExportSongsAboveDuration(context, 4);

        Console.WriteLine(res);
    }

    public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
    {
        var albumsInfo = context.Albums
            .Where(a => a.ProducerId.HasValue &&
                        a.ProducerId.Value == producerId)
            .ToArray()
            .OrderByDescending(a => a.Price)
            .Select(a => new
            {
                a.Name,
                ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                ProducerName = a.Producer!.Name,
                Songs = a.Songs
                        .Select(s => new
                        {
                            SongName = s.Name,
                            Price = s.Price.ToString("f2"),
                            Writer = s.Writer.Name
                        })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.Writer)
                        .ToArray(),
                AlbumPrice = a.Price.ToString("f2")
            })
            .ToArray();

        var sb = new StringBuilder();

        foreach (var a in albumsInfo)
        {
            sb.AppendLine($"-AlbumName: {a.Name}")
              .AppendLine($"-ReleaseDate: {a.ReleaseDate}")
              .AppendLine($"-ProducerName: {a.ProducerName}")
              .AppendLine($"-Songs:");
            int i = 1;
            foreach (var s in a.Songs)
            {
                sb.AppendLine($"---#{i}")
                  .AppendLine($"---SongName: {s.SongName}")
                  .AppendLine($"---Price: {s.Price}")
                  .AppendLine($"---Writer: {s.Writer}");
                i++;
            }
            sb.AppendLine($"-AlbumPrice: {a.AlbumPrice}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
    {
        var songsInfo = context.Songs
            .ToArray()
            .Where(s => s.Duration.TotalSeconds > duration)
            .Select(s => new
            {
                s.Name,
                Performers = s.SongPerformers
                            .Select(p => $"{p.Performer.FirstName} {p.Performer.LastName}")
                            .OrderBy(p => p)
                            .ToArray(),
                WriterName = s.Writer.Name,
                AlbumProducer = s.Album!.Producer!.Name,
                Duration = s.Duration.ToString("c")
            })
            .OrderBy(s => s.Name)
            .ThenBy(s => s.WriterName)
            .ToArray();

        var sb = new StringBuilder();
        int i = 1;

        foreach (var s in songsInfo)
        {
            sb.AppendLine($"-Song #{i}")
              .AppendLine($"---SongName: {s.Name}")
              .AppendLine($"---Writer: {s.WriterName}");

            foreach (var p in s.Performers)
            {
                sb.AppendLine($"---Performer: {p}");
            }

            sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}")
              .AppendLine($"---Duration: {s.Duration}");

            i++;
        }
        return sb.ToString().TrimEnd();
    }
}