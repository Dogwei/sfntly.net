using com.google.typography.font.sfntly;
using com.google.typography.font.sfntly.data;
using com.google.typography.font.sfntly.table.core;
using com.google.typography.font.tools.sfnttool;
using com.google.typography.font.tools.subsetter;
using System;
using System.Collections.Generic;
using System.IO;

namespace sfntly.console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputFile = new FileInfo(@"C:/Windows/Fonts/simkai.ttf");
            var outFile = new FileInfo("D:/out.ttf");

            var sfntTool = new SfntTool();

            sfntTool.subsetString = "abcABC123中国";
            sfntTool.strip = true;

            sfntTool.subsetFontFile(inputFile, outFile, 1);
        }
    }
}