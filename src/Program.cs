using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Cap2Srt
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: Cap2Srt --input <input-file.sbv> [--output <output-file.srt>]");
                Console.WriteLine("   or: Cap2Srt -i <input-file.sbv> [-o <output-file.srt>]");
                return 1;
            }

            string? inputFile = null;
            string? outputFile = null;

            // Simple argument parsing
            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == "--input" || args[i] == "-i") && i + 1 < args.Length)
                {
                    inputFile = args[i + 1];
                    i++; // Skip the next arg
                }
                else if ((args[i] == "--output" || args[i] == "-o") && i + 1 < args.Length)
                {
                    outputFile = args[i + 1];
                    i++; // Skip the next arg
                }
            }

            // Validate arguments
            if (string.IsNullOrEmpty(inputFile))
            {
                Console.Error.WriteLine("Error: Input file must be specified with --input or -i");
                return 1;
            }

            // If output is not specified, use the input file name with .srt extension
            if (string.IsNullOrEmpty(outputFile))
            {
                outputFile = Path.ChangeExtension(inputFile, ".srt");
            }

            try
            {
                if (!File.Exists(inputFile))
                {
                    Console.Error.WriteLine($"Error: Input file {inputFile} does not exist.");
                    return 1;
                }

                await ConvertSbvToSrt(inputFile, outputFile);
                Console.WriteLine($"Successfully converted {Path.GetFileName(inputFile)} to {Path.GetFileName(outputFile)}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        static async Task ConvertSbvToSrt(string inputFile, string outputFile)
        {
            string[] lines = await File.ReadAllLinesAsync(inputFile);
            
            using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);
            
            int subtitleIndex = 1;
            int lineIndex = 0;
            
            while (lineIndex < lines.Length)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(lines[lineIndex]))
                {
                    lineIndex++;
                    continue;
                }

                // Parse timestamp line (format: h:mm:ss.ttt,h:mm:ss.ttt)
                string timestampLine = lines[lineIndex];
                lineIndex++;
                
                var match = Regex.Match(timestampLine, @"(\d+:\d+:\d+\.\d+),(\d+:\d+:\d+\.\d+)");
                if (!match.Success)
                {
                    continue; // Skip if not a timestamp line
                }
                
                string startTime = match.Groups[1].Value;
                string endTime = match.Groups[2].Value;
                
                // Convert timestamps to SRT format (hh:mm:ss,ttt)
                string srtStartTime = ConvertTimestamp(startTime);
                string srtEndTime = ConvertTimestamp(endTime);
                
                // Write subtitle index
                await writer.WriteLineAsync(subtitleIndex.ToString());
                
                // Write timestamp line in SRT format
                await writer.WriteLineAsync($"{srtStartTime} --> {srtEndTime}");
                
                // Collect and write subtitle content until an empty line or another timestamp
                StringBuilder content = new StringBuilder();
                
                while (lineIndex < lines.Length && 
                       !string.IsNullOrWhiteSpace(lines[lineIndex]) && 
                       !Regex.IsMatch(lines[lineIndex], @"\d+:\d+:\d+\.\d+,\d+:\d+:\d+\.\d+"))
                {
                    content.AppendLine(lines[lineIndex]);
                    lineIndex++;
                }
                
                await writer.WriteLineAsync(content.ToString().TrimEnd());
                await writer.WriteLineAsync(); // Add blank line between entries
                
                subtitleIndex++;
            }
        }

        static string ConvertTimestamp(string sbvTimestamp)
        {
            // Parse SBV timestamp format (h:mm:ss.ttt) to SRT format (hh:mm:ss,ttt)
            if (TimeSpan.TryParse(sbvTimestamp, CultureInfo.InvariantCulture, out TimeSpan time))
            {
                // Format to SRT timestamp (replace . with , for milliseconds)
                return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2},{time.Milliseconds:D3}";
            }
            
            return sbvTimestamp.Replace('.', ',');
        }
    }
}
