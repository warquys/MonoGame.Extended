// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Content.Tiled;

/// <summary>
/// Represents an image element in a Tiled map, containing source, dimensions, format, transparency color, and tile data.
/// </summary>
public class TiledMapImageContent
{
    //[XmlIgnore]
    //public Texture2DContent Content { get; set; }

    //[XmlIgnore]
    //public ExternalReference<Texture2DContent> ContentRef { get; set; }

    /// <summary>
    /// Gets or sets the source path of the image.
    /// </summary>
    [XmlAttribute(AttributeName = "source")]
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the width of the image in pixels.
    /// </summary>
    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the image in pixels.
    /// </summary>
    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the format of the image (e.g., "png", "gif").
    /// </summary>
    [XmlAttribute(AttributeName = "format")]
    public string Format { get; set; }

    /// <summary>
    /// Gets or sets the raw string representation of the transparent color, as stored in the XML attribute.
    /// </summary>
    [XmlAttribute(AttributeName = "trans")]
    public string RawTransparentColor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transparent color of the image.
    /// Converts between the hex string representation and <see cref="Color"/>.
    /// </summary>
    [XmlIgnore]
    public Color TransparentColor
    {
        get => RawTransparentColor == string.Empty ? Color.Transparent : ColorHelper.FromHex(RawTransparentColor);
        set => RawTransparentColor = value.ToHex();
    }

    /// <summary>
    /// Gets or sets the tile data associated with the image.
    /// </summary>
    [XmlElement(ElementName = "data")]
    public TiledMapTileLayerDataContent Data { get; set; }

    /// <summary>
    /// Returns the source path as the string representation.
    /// </summary>
    /// <returns>The source path of the image.</returns>
    public override string ToString()
    {
        return Source;
    }
}
