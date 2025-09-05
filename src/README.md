# Cap2Srt - SBV to SRT Caption Converter

A simple, lightweight command-line utility to convert SubViewer (SBV) caption files to SubRip (SRT) format.

## Features

- Converts SBV captions to SRT format
- Simple command-line interface
- Lightweight and fast
- Cross-platform (.NET 9)
- Dockerized for easy deployment

## Usage

### Direct .NET Usage

```bash
# Basic usage (output will be named the same as input but with .srt extension)
dotnet Cap2Srt.dll --input video_captions.sbv

# Specify output file
dotnet Cap2Srt.dll --input video_captions.sbv --output my_captions.srt

# Using aliases
dotnet Cap2Srt.dll -i video_captions.sbv -o my_captions.srt
```

### Docker Usage

```bash
# Build the Docker image
docker build -t cap2srt .

# Run the Docker container
# Note: You need to mount a volume to access your local files
docker run --rm -v $(pwd):/data cap2srt --input /data/video_captions.sbv --output /data/my_captions.srt
```

## SBV vs SRT Format

### SBV Format (Input)

```plaintext
0:00:00.000,0:00:05.000
This is the first subtitle

0:00:05.500,0:00:10.000
This is the second subtitle
that spans multiple lines
```

### SRT Format (Output)

```plaintext
1
00:00:00,000 --> 00:00:05,000
This is the first subtitle

2
00:00:05,500 --> 00:00:10,000
This is the second subtitle
that spans multiple lines
```

## License

MIT
