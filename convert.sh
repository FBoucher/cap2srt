#!/bin/bash

# Build the Docker image
docker build -t cap2srt ./Cap2Srt

# Check if input file parameter is provided
if [ -z "$1" ]; then
    echo "Usage: $0 <input-sbv-file> [output-srt-file]"
    echo "Example: $0 captions.sbv output.srt"
    exit 1
fi

INPUT_FILE="$1"
OUTPUT_FILE="$2"

# If output file is not provided, use the same name as input with .srt extension
if [ -z "$OUTPUT_FILE" ]; then
    OUTPUT_FILE="${INPUT_FILE%.*}.srt"
fi

# Check if input file exists
if [ ! -f "$INPUT_FILE" ]; then
    echo "Error: Input file '$INPUT_FILE' not found"
    exit 1
fi

# Get absolute path of input and output files
INPUT_PATH="$(cd "$(dirname "$INPUT_FILE")" && pwd)/$(basename "$INPUT_FILE")"
OUTPUT_DIR="$(dirname "$OUTPUT_FILE")"
OUTPUT_PATH="$(cd "$OUTPUT_DIR" 2>/dev/null && pwd || mkdir -p "$OUTPUT_DIR" && cd "$OUTPUT_DIR" && pwd)/$(basename "$OUTPUT_FILE")"

# Run the converter using Docker
echo "Converting $INPUT_PATH to $OUTPUT_PATH"
docker run --rm -v "$(dirname "$INPUT_PATH"):/input" -v "$(dirname "$OUTPUT_PATH"):/output" \
    cap2srt --input "/input/$(basename "$INPUT_PATH")" --output "/output/$(basename "$OUTPUT_PATH")"

# Check if conversion was successful
if [ $? -eq 0 ]; then
    echo "Conversion completed successfully"
else
    echo "Conversion failed"
fi
