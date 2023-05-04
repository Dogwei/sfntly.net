// Copyright 2012 Google Inc. All Rights Reserved.

namespace com.google.typography.font.tools.fontinfo;








/**
 * This is the main class for the command-line version of the font info tool
 *
 * @author Han-Wen Yeh
 *
 */
public class FontInfoMain {
  private static readonly String PROGRAM_NAME = "java -jar fontinfo.jar";

  public static void main(String[] args) {
    CommandOptions options = new CommandOptions();
    JCommander commander = null;
    try {
      commander = new JCommander(options, args);
    } catch (ParameterException e) {
      Console.WriteLine(e.getMessage());
      commander = new JCommander(options, "--help");
    }

    // Display help
    if (options.help) {
      commander.setProgramName(PROGRAM_NAME);
      commander.usage();
      return;
    }

    // No font loaded
    if (options.files.size() != 1) {
      Console.WriteLine(
          "Please specify a single font. Try '" + PROGRAM_NAME + " --help' for more information.");
      return;
    }

    // Default option
    if (!(options.metrics || options.general || options.cmap || options.chars || options.blocks
        || options.scripts || options.glyphs || options.all)) {
      options.general = true;
    }

    // Obtain file name
    String fileName = options.files.get(0);

    // Load font
    Font[] fonts = null;
    try {
      fonts = FontUtils.getFonts(fileName);
    } catch (IOException e) {
      Console.WriteLine("Unable to load font " + fileName);
      return;
    }

    for (int i = 0; i < fonts.length; i++) {
      Font font = fonts[i];

      if (fonts.length > 1 && !options.csv) {
        Console.WriteLine("==== Information for font index " + i + " ====\n");
      }

      // Print general information
      if (options.general || options.all) {
        if (options.csv) {
          Console.WriteLine(String.Format("sfnt version: %s", FontInfo.sfntVersion(font)));
          Console.WriteLine();
          Console.WriteLine("Font Tables");
          Console.WriteLine(
              prependDataAndBuildCsv(FontInfo.listTables(font).csvStringArray(), fileName, i));
          Console.WriteLine();
          Console.WriteLine("Name Table Entries:");
          Console.WriteLine(
              prependDataAndBuildCsv(FontInfo.listNameEntries(font).csvStringArray(), fileName, i));
          Console.WriteLine();
        } else {
          Console.WriteLine(String.Format("sfnt version: %s", FontInfo.sfntVersion(font)));
          Console.WriteLine();
          Console.WriteLine("Font Tables:");
          FontInfo.listTables(font).prettyPrint();
          Console.WriteLine();
          Console.WriteLine("Name Table Entries:");
          FontInfo.listNameEntries(font).prettyPrint();
          Console.WriteLine();
        }
      }

      // Print metrics
      if (options.metrics || options.all) {
        if (options.csv) {
          Console.WriteLine("Font Metrics:");
          Console.WriteLine(
              prependDataAndBuildCsv(FontInfo.listFontMetrics(font).csvStringArray(), fileName, i));
          Console.WriteLine();
        } else {
          Console.WriteLine("Font Metrics:");
          FontInfo.listFontMetrics(font).prettyPrint();
          Console.WriteLine();
        }
      }

      // Print glyph metrics
      if (options.metrics || options.glyphs || options.all) {
        if (options.csv) {
          Console.WriteLine("Glyph Metrics:");
          Console.WriteLine(prependDataAndBuildCsv(
              FontInfo.listGlyphDimensionBounds(font).csvStringArray(), fileName, i));
          Console.WriteLine();
        } else {
          Console.WriteLine("Glyph Metrics:");
          FontInfo.listGlyphDimensionBounds(font).prettyPrint();
          Console.WriteLine();
        }
      }

      // Print cmap list
      if (options.cmap || options.all) {
        if (options.csv) {
          Console.WriteLine("Cmaps in the font:");
          Console.WriteLine(
              prependDataAndBuildCsv(FontInfo.listCmaps(font).csvStringArray(), fileName, i));
          Console.WriteLine();
        } else {
          Console.WriteLine("Cmaps in the font:");
          FontInfo.listCmaps(font).prettyPrint();
          Console.WriteLine();
        }
      }

      // Print blocks
      if (options.blocks || options.all) {
        if (options.csv) {
          Console.WriteLine("Unicode block coverage:");
          Console.WriteLine(prependDataAndBuildCsv(
              FontInfo.listCharBlockCoverage(font).csvStringArray(), fileName, i));
          Console.WriteLine();
        } else {
          Console.WriteLine("Unicode block coverage:");
          FontInfo.listCharBlockCoverage(font).prettyPrint();
          Console.WriteLine();
        }
      }

      // Print scripts
      if (options.scripts || options.all) {
        if (options.csv) {
          Console.WriteLine("Unicode script coverage:");
          Console.WriteLine(prependDataAndBuildCsv(
              FontInfo.listScriptCoverage(font).csvStringArray(), fileName, i));
          Console.WriteLine();
          if (options.detailed) {
            Console.WriteLine("Uncovered code points in partially-covered scripts:");
            Console.WriteLine(prependDataAndBuildCsv(
                FontInfo.listCharsNeededToCoverScript(font).csvStringArray(), fileName, i));
            Console.WriteLine();
          }
        } else {
          Console.WriteLine("Unicode script coverage:");
          FontInfo.listScriptCoverage(font).prettyPrint();
          Console.WriteLine();
          if (options.detailed) {
            Console.WriteLine("Uncovered code points in partially-covered scripts:");
            FontInfo.listCharsNeededToCoverScript(font).prettyPrint();
            Console.WriteLine();
          }
        }
      }

      // Print char list
      if (options.chars || options.all) {
        if (options.csv) {
          Console.WriteLine("Characters with valid glyphs:");
          Console.WriteLine(
              prependDataAndBuildCsv(FontInfo.listChars(font).csvStringArray(), fileName, i));
          Console.WriteLine();
        } else {
          Console.WriteLine("Characters with valid glyphs:");
          FontInfo.listChars(font).prettyPrint();
          Console.WriteLine();
          Console.WriteLine(String.format(
              "Total number of characters with valid glyphs: %d", FontInfo.numChars(font)));
          Console.WriteLine();
        }
      }

      // Print glyph information
      if (options.glyphs || options.all) {
        DataDisplayTable unmappedGlyphs = FontInfo.listUnmappedGlyphs(font);
        if (options.csv) {
          Console.WriteLine(String.Format("Total hinting size: %s", FontInfo.hintingSize(font)));
          Console.WriteLine(String.format(
              "Number of unmapped glyphs: %d / %d", unmappedGlyphs.getNumRows(),
              FontInfo.numGlyphs(font)));
          Console.WriteLine();
          if (options.detailed) {
            Console.WriteLine("Unmapped glyphs:");
            Console.WriteLine(
                prependDataAndBuildCsv(unmappedGlyphs.csvStringArray(), fileName, i));
            Console.WriteLine();
          }
          Console.WriteLine("Subglyphs used by characters in the font:");
          Console.WriteLine(prependDataAndBuildCsv(
              FontInfo.listSubglyphFrequency(font).csvStringArray(), fileName, i));
          Console.WriteLine();
        } else {
          Console.WriteLine(String.Format("Total hinting size: %s", FontInfo.hintingSize(font)));
          Console.WriteLine(String.format(
              "Number of unmapped glyphs: %d / %d", unmappedGlyphs.getNumRows(),
              FontInfo.numGlyphs(font)));
          Console.WriteLine();
          if (options.detailed) {
            Console.WriteLine("Unmapped glyphs:");
            unmappedGlyphs.prettyPrint();
            Console.WriteLine();
          }
          Console.WriteLine("Subglyphs used by characters in the font:");
          FontInfo.listSubglyphFrequency(font).prettyPrint();
          Console.WriteLine();
        }
      }
    }
  }

  private static String prependDataAndBuildCsv(String[] arr, String fontName, int fontIndex) {
    StringBuilder output = new StringBuilder("Font,font index,").append(arr[0]).append('\n');
    for (int i = 1; i < arr.length; i++) {
      String row = arr[i];
      output.append(fontName)
          .append(',')
          .append("font index ")
          .append(fontIndex)
          .append(',')
          .append(row)
          .append('\n');
    }
    return output.ToString();
  }
}
