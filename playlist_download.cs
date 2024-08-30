using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {    string directoryPath = @"Downloaded_Videos";
            string[] files = Directory.GetFiles(directoryPath);
            int fileCount = files.Length;
            var youtube = new YoutubeClient();
            var playlistUrl = "https://youtube.com/playlist?list=PLFjydPMg4Dapq9vcdmGyHs8uJhiqMgUrX&si=DZB-brUY3If5ZXc7";
            var downloadFolder = "Downloaded_Videos";
            var start = fileCount;
            var i = 0;
            Directory.CreateDirectory(downloadFolder);
            Console.WriteLine($"There are {fileCount} videos right now");
            await foreach (var video in youtube.Playlists.GetVideosAsync(playlistUrl))
            {
            if (i>=start){
                var videoId = video.Id;
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);

                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                if (streamInfo == null)
                {
                    Console.WriteLine($"No muxed streams available for video: {video.Title}");
                    continue;
                }

                // Define the file name and path
                var safeTitle = string.Concat(video.Title.Split(Path.GetInvalidFileNameChars()));
                var fileName = $"{safeTitle}.{streamInfo.Container.Name.ToLower()}";
                var filePath = Path.Combine(downloadFolder, fileName);

                // Download the video
                using var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await stream.CopyToAsync(fileStream);

                Console.WriteLine($"Downloaded: {video.Title}");
            }
            i++;}
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
