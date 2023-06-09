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
using System.Reflection.PortableExecutable;

namespace com.google.typography.font.sfntly.table;

/**
 * A table builder to do the minimal table building for an unknown table type.
 *
 * @author Stuart Gill
 *
 */
internal sealed class GenericTableBuilder : TableBasedTableBuilder<Table>
{
    /**
     * Create a new builder using the header information and data provided.
     *
     * @param header the header information
     * @param data the data holding the table
     * @return a new builder
     */
    public static GenericTableBuilder createBuilder(Header header, WritableFontData data)
    {
        return new GenericTableBuilder(header, data);
    }

    /**
     * @param header
     * @param data
     */
    private GenericTableBuilder(Header header, WritableFontData data) : base(header, data)
    {

    }

    /**
     * @param header
     * @param data
     */
    private GenericTableBuilder(Header header, ReadableFontData data) : base(header, data)
    {

    }

    public override Table subBuildTable(ReadableFontData data)
    {
        return new Table(this.header(), this.internalReadData());
    }
}