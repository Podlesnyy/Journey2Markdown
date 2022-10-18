using System.Globalization;
using System.Text;
using System.Text.Json;
using Convertor.JourneyJsons;

namespace Convertor;

internal static class Program
{
    private const string resultDir = @"g:\Temp\MD";

    private static void Main(string[] args)
    {
        if (Directory.Exists(resultDir))
            Directory.Delete(resultDir, true);

        Console.WriteLine("Hello, World!");
        foreach (var file in Directory.GetFiles(@"g:\Temp\journey-multiple\", "*.json"))
        {
            var content = File.ReadAllText(file);
            var record = JsonSerializer.Deserialize<JourneyRecord>(content);

            var time = GetTime(record.date_journal, record.timezone);

            var recordDir = GetDirectoryForMarkdownRecord(time);
            Directory.CreateDirectory(recordDir);
            var fileName = $"{time.Year}-{time.Month.ToString().PadLeft(2, '0')}-{time.Day.ToString().PadLeft(2, '0')}";

            var sb = new StringBuilder();

            sb.AppendLine( new Html2Markdown.Converter().Convert(record.text) );

            sb.AppendLine();

            if (!string.IsNullOrEmpty(record.address))
                sb.AppendLine($"address={record.address}");

            if (record.lat < 360)
                sb.AppendLine($"lat={record.lat.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}lon={record.lon.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}https://www.google.com/maps/place/{GetCoord(record.lat, 'N', 'S')}+{GetCoord(record.lon, 'E', 'W')}");

            var photoIndex = 0;
            foreach (var photoFile in record.photos)
            {
                File.Copy(Path.Combine( Path.GetDirectoryName(file), photoFile.ToString()), Path.Combine(recordDir, $"{fileName}_{photoIndex}{Path.GetExtension(photoFile.ToString())}"));
                photoIndex++;
            }
            //sb.AppendLine(content);
            File.WriteAllText(Path.Combine(recordDir, $"{fileName}.md"), sb.ToString(), Encoding.UTF8);
        }
    }

    private static string GetCoord(float coord, char positive, char negative)
    {
        var ts = TimeSpan.FromHours(Math.Abs(coord));
        var degrees = Convert.ToInt32(Math.Floor(ts.TotalHours));
        return $"{degrees}%C2%B0{ts.Minutes}'{ts.Seconds}.{ts.Milliseconds}%22{(Math.Sign(coord) > 0 ? positive : negative)}";
    }

    private static string GetDirectoryForMarkdownRecord(DateTime time)
    {
        var monthDir = $"{time.Month.ToString().PadLeft(2, '0')} {DateTimeFormatInfo.CurrentInfo.GetMonthName(time.Month)}";
        var dayDir = time.Day switch
        {
            <= 10 => "1-10",
            > 10 and < 21 => "11-20",
            _ => "21-31"
        };

        return Path.Combine(new[] { resultDir, time.Year.ToString(), monthDir, dayDir });
    }

    private static DateTime GetTime(long recordDateJournal, string recordTimezone)
    {
        if (string.IsNullOrEmpty(recordTimezone)) recordTimezone = "Europe/Moscow";

        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UnixEpoch.AddMilliseconds(recordDateJournal), recordTimezone);
    }
}