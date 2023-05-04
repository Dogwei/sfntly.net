/*
 * Copyright 2011 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using com.google.typography.font.sfntly.data;
using static com.google.typography.font.sfntly.table.bitmap.EblcTable;

namespace com.google.typography.font.sfntly.table.bitmap;











/**
 * Format 3 Index Subtable Entry.
 *
 * @author Stuart Gill
 *
 */
public sealed class IndexSubTableFormat3 : IndexSubTable
{
    private IndexSubTableFormat3(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
    {
    }

    public override int numGlyphs()
    {
        return this.lastGlyphIndex() - this.firstGlyphIndex() + 1;
    }

    public override int glyphStartOffset(int glyphId)
    {
        int loca = this.checkGlyphRange(glyphId);
        return this.loca(loca);
    }

    public override int glyphLength(int glyphId)
    {
        int loca = this.checkGlyphRange(glyphId);
        return this.loca(loca + 1) - this.loca(loca);
    }

    private int loca(int loca)
    {
        int readLocation =
            (int)Offset.indexSubTable3_offsetArray + loca * (int)FontData.DataSize.USHORT;
        int readOffset = this._data.readUShort(
            (int)Offset.indexSubTable3_offsetArray + loca * (int)FontData.DataSize.USHORT);
        return readOffset;
    }

    public static IBuilder createBuilder()
    {
        return new Builder();
    }

    public static IBuilder createBuilder(
    ReadableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
    {
        int length = Builder.dataLength(data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
        return new Builder(data.slice(indexSubTableOffset, length), firstGlyphIndex, lastGlyphIndex);
    }

    public static IBuilder createBuilder(
    WritableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
    {
        int length = Builder.dataLength(data, indexSubTableOffset, firstGlyphIndex, lastGlyphIndex);
        return new Builder(data.slice(indexSubTableOffset, length), firstGlyphIndex, lastGlyphIndex);
    }

    public interface IBuilder : IndexSubTable.IBuilder<IndexSubTableFormat3>
    {

    }

    private sealed class Builder : IndexSubTable.Builder<IndexSubTableFormat3>, IBuilder
    {
        private IList<Integer> _offsetArray;

        public static int dataLength(
            ReadableFontData data, int indexSubTableOffset, int firstGlyphIndex, int lastGlyphIndex)
        {
            return (int)Offset.indexSubHeaderLength + (lastGlyphIndex - firstGlyphIndex + 1 + 1)
                * (int)FontData.DataSize.USHORT;
        }

        public Builder() : base((int)Offset.indexSubTable3_builderDataSize, Format.FORMAT_3)
        {
        }

        public Builder(WritableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
        {
        }

        public Builder(ReadableFontData data, int firstGlyphIndex, int lastGlyphIndex) : base(data, firstGlyphIndex, lastGlyphIndex)
        {
        }

        public override int numGlyphs()
        {
            return this.getOffsetArray().Count - 1;
        }

        public override int glyphLength(int glyphId)
        {
            int loca = this.checkGlyphRange(glyphId);
            IList<Integer> offsetArray = this.getOffsetArray();
            return offsetArray.get(loca + 1) - offsetArray.get(loca);
        }

        public override int glyphStartOffset(int glyphId)
        {
            int loca = this.checkGlyphRange(glyphId);
            IList<Integer> offsetArray = this.getOffsetArray();
            return offsetArray.get(loca);
        }

        public IList<Integer> offsetArray()
        {
            return this.getOffsetArray();
        }

        private IList<Integer> getOffsetArray()
        {
            if (this.offsetArray == null)
            {
                this.initialize(base.internalReadData());
                base.setModelChanged();
            }
            return this._offsetArray;
        }

        private void initialize(ReadableFontData data)
        {
            if (this._offsetArray == null)
            {
                this._offsetArray = new List<Integer>();
            }
            else
            {
                this._offsetArray.Clear();
            }

            if (data != null)
            {
                int numOffsets = (this.lastGlyphIndex() - this.firstGlyphIndex() + 1) + 1;
                for (int i = 0; i < numOffsets; i++)
                {
                    this._offsetArray.Add(data.readUShort(
(int)Offset.indexSubTable3_offsetArray + i
              * (int)FontData.DataSize.USHORT));
                }
            }
        }

        public void setOffsetArray(IList<Integer> array)
        {
            this._offsetArray = array;
            this.setModelChanged();
        }

        /*private class BitmapGlyphInfoIterator : IEnumerator<BitmapGlyphInfo>
        {
            private int glyphId;

            public BitmapGlyphInfoIterator()
            {
                this.glyphId = IndexSubTableFormat3.Builder.@this.firstGlyphIndex();
            }

            public override boolean hasNext()
            {
                if (this.glyphId <= IndexSubTableFormat3.Builder.@this.lastGlyphIndex())
                {
                    return true;
                }
                return false;
            }

            public override BitmapGlyphInfo next()
            {
                if (!hasNext())
                {
                    throw new NoSuchElementException("No more characters to iterate.");
                }
                BitmapGlyphInfo info =
                    new BitmapGlyphInfo(this.glyphId, IndexSubTableFormat3.Builder.@this.imageDataOffset(),
                        IndexSubTableFormat3.Builder.@this.glyphStartOffset(this.glyphId),
                        IndexSubTableFormat3.Builder.@this.glyphLength(this.glyphId),
                        IndexSubTableFormat3.Builder.@this.imageFormat());
                this.glyphId++;
                return info;
            }

            public override void remove()
            {
                throw new UnsupportedOperationException("Unable to remove a glyph info.");
            }
        }*/

        public override IEnumerator<BitmapGlyphInfo> GetEnumerator()
        {
            return Enumerable
                .Range(firstGlyphIndex(), lastGlyphIndex() - firstGlyphIndex() + 1)
                .Select(glyphId => new BitmapGlyphInfo(glyphId, imageDataOffset(), glyphStartOffset(glyphId), glyphLength(glyphId), imageFormat()))
                .GetEnumerator();
            /*return new BitmapGlyphInfoIterator();*/
        }

        public override void revert()
        {
            base.revert();
            this._offsetArray = null;
        }

        public override IndexSubTableFormat3 subBuildTable(ReadableFontData data)
        {
            return new IndexSubTableFormat3(data, this.firstGlyphIndex(), this.lastGlyphIndex());
        }

        public override void subDataSet()
        {
            this.revert();
        }

        public override int subDataSizeToSerialize()
        {
            if (this.offsetArray == null)
            {
                return this.internalReadData().length();
            }
            return (int)Offset.indexSubHeaderLength + this._offsetArray.Count
                * (int)FontData.DataSize.ULONG;
        }

        public override boolean subReadyToSerialize()
        {
            if (this.offsetArray != null)
            {
                return true;
            }
            return false;
        }

        public override int subSerialize(WritableFontData newData)
        {
            int size = base.serializeIndexSubHeader(newData);
            if (!this.modelChanged())
            {
                size += this.internalReadData().slice((int)Offset.indexSubTable3_offsetArray).copyTo(
                    newData.slice((int)Offset.indexSubTable3_offsetArray));
            }
            else
            {

                foreach (Integer loca in this._offsetArray)
                {
                    size += newData.writeUShort(size, loca);
                }
            }
            return size;
        }
    }
}