#!/bin/bash

timestamp=$(date +%s)
echo $timestamp


target=$1
xbase=${target##*/}
target_name=${xbase%.*}

path="/Users/Mac_Alex/Library/Application Support/Steam/steamapps/common/Unturned/Servers/Test/Rocket/Plugins/"
filename="$path""$target_name"_"$timestamp".dll

echo $path


cd "$path"
find . -name "$target_name*.dll" -type f -delete



cp "$target" "$filename"
