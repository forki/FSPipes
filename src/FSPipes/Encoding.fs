﻿(*
   Copyright 2015-2017 Philip Curzon

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*)

namespace NovelFS.FSPipes

/// Defines possible endianness options
type Endianness =
    /// Big Endian
    |BigEndian
    /// Little Endian
    |LittleEndian

/// UTF8 encoding options
type UTF8EncodingOptions = {EmitIdentifier : bool}
/// Unicode encoding options
type UTF16EncodingOptions = {Endianness : Endianness; ByteOrderMark : bool}
/// UTF32 encoding options
type UTF32EncodingOptions = {Endianness : Endianness; ByteOrderMark : bool}

/// Represents a set of possible text encodings with options if applicable
type Encoding = 
    /// Ascii encoding
    |Ascii
    /// UTF-7 encoding
    |UTF7
    /// UTF-8 encoding with specified encoding options
    |UTF8 of UTF8EncodingOptions
    /// UTF-16 encoding with specified encoding options
    |UTF16 of UTF16EncodingOptions
    /// UTF-32 encoding with specified encoding options
    |UTF32 of UTF32EncodingOptions

/// Functions for handling byte order
module ByteOrder =
    /// Gets the endianness of the current platform
    let systemEndianness =
        match System.BitConverter.IsLittleEndian with
        |true -> LittleEndian
        |false -> BigEndian

    /// Returns true if the supplied endianness is big endian
    let isBigEndian endianness =
        match endianness with
        |BigEndian -> true
        |_ -> false

/// A set of text encoding related types and function
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Encoding =
    let utf8WithoutBom = UTF8 {EmitIdentifier = false}

    let utf8Bom = UTF8 {EmitIdentifier = true}

    /// Creates a .NET encoding for the supplied encoding, using the supplied endianness if it's not specified in the encoding
    let createDotNetEncoding enc =
        match enc with
        |Ascii -> System.Text.Encoding.ASCII
        |UTF7 -> System.Text.Encoding.UTF7
        |UTF8 {EmitIdentifier = ident} -> 
            System.Text.UTF8Encoding(ident) :> System.Text.Encoding
        |UTF16 {Endianness = endianness; ByteOrderMark = mark} -> 
            System.Text.UnicodeEncoding(ByteOrder.isBigEndian  endianness, mark) :> System.Text.Encoding
        |UTF32 {Endianness = endianness; ByteOrderMark = mark} -> 
            System.Text.UTF32Encoding(ByteOrder.isBigEndian  endianness, mark) :> System.Text.Encoding

    /// Gets the length of the supplied string in the supplied encoding
    let byteLength encoding (str : string) =
        let enc = createDotNetEncoding  encoding
        enc.GetByteCount str

    /// Gets the length of the preamble in the supplied encoding
    let preambleLength encoding =
        Array.length <| (createDotNetEncoding encoding).GetPreamble()

    /// Gets the preamble in the supplied encoding
    let preamble encoding =
        (createDotNetEncoding encoding).GetPreamble()
