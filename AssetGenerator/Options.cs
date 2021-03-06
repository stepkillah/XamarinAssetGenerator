﻿using System;
using System.Collections.Generic;
using System.IO;
using AssetGenerator.Enums;
using CommandLine;
using CommandLine.Text;

namespace AssetGenerator
{
    public class Options
    {
        [Option('m', "mode", HelpText = "Specify which assets to build. ios or android or appicon", Required = false, Default = "ios")]
        public string Mode { get; set; }

        [Option('s', "source", Separator = '-', HelpText = "Specify source folder", Required = false)]
        public string SourceFolderPath { get; set; }

        [Option('d', "destination", Separator = '-', HelpText = "Specify destination folder", Required = false)]
        public string DestinationFolderPath { get; set; } = Directory.GetCurrentDirectory();

        [Option('q', "quality", Separator = '-', HelpText = "Specify quality of rendered png files (100 is max)", Required = false, Default = 80)]
        public int Quality { get; set; }
        
        [Option('p', "postifx", Separator = '-', HelpText = "Specify the desired api postfix for Android (v4, v17) ", Required = false)]
        public string Postfix { get; set; }

        [Option('v', "version", Separator = '-', HelpText = "Display version information", Required = false)]
        public bool Version { get; set; }

        public GeneratorMode MappedMode => Enum.Parse<GeneratorMode>(Mode, true);

        public bool ShouldProceed { get; set; } = true;

        [Usage(ApplicationAlias = "AssetGenerator")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Common usage",
                    new Options
                    {
                        Mode = "iOS",
                        SourceFolderPath = "sourcefolder",
                        DestinationFolderPath = "destinationfolder",
                        Postfix = string.Empty,
                        Quality = 80
                    });
            }
        }
    }
}