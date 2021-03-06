﻿using System.ComponentModel;
using System.Configuration;
using System.IO;
using TextModel.Interfaces;

namespace TextModel.FileService
{
    public class FileService: IFileService
    {
        
        public StreamReader GetStreamReader(string openPath)
        {
            var reader = new StreamReader(openPath);
            return reader;
        }

        public void Save(IText text,string closePath)
        {
            using (var streamWriter = new StreamWriter(closePath))
            {
                foreach (var sentence in text.Sentences)
                {
                    foreach (var item in sentence.Items)
                    {
                        streamWriter.Write(item.Chars);
                    }
                }
            }
        }
    }
}