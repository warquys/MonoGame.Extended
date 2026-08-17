using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.ViewportAdapters
{
    /// <summary>
    /// Provides an abstraction for adapting a virtual viewport to a graphics device viewport.
    /// Handles coordinate conversion between screen and virtual coordinates, exposes viewport
    /// dimensions, and provides the scale matrix used for rendering.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device used for rendering.</param>
    public abstract class ViewportAdapter(GraphicsDevice graphicsDevice) : IDisposable
    {
        #region Properties & Variables

        /// <summary>
        /// Gets the bounding rectangle of the virtual viewport, starting at (0,0) with dimensions
        /// <see cref="VirtualWidth"/> × <see cref="VirtualHeight"/>.
        /// </summary>
        public Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);

        /// <summary>
        /// Gets the center point of the virtual viewport.
        /// </summary>
        public Point Center => BoundingRectangle.Center;

        /// <summary>
        /// Gets the graphics device associated with this adapter.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; } = graphicsDevice;

        /// <summary>
        /// Gets the current viewport of the graphics device.
        /// </summary>
        public Viewport Viewport => GraphicsDevice.Viewport;

        /// <summary>
        /// Gets the height of the graphics device viewport in pixels.
        /// </summary>
        public abstract int ViewportHeight { get; }

        /// <summary>
        /// Gets the width of the graphics device viewport in pixels.
        /// </summary>
        public abstract int ViewportWidth { get; }

        /// <summary>
        /// Gets the height of the virtual viewport.
        /// </summary>
        public abstract int VirtualHeight { get; }

        /// <summary>
        /// Gets the width of the virtual viewport.
        /// </summary>
        public abstract int VirtualWidth { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources used by the <see cref="ViewportAdapter"/>.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Gets the scale matrix that transforms virtual coordinates to screen coordinates.
        /// </summary>
        /// <returns>A <see cref="Matrix"/> representing the scale transformation.</returns>
        public abstract Matrix GetScaleMatrix();

        /// <summary>
        /// Converts a point from virtual coordinates to screen coordinates.
        /// </summary>
        /// <param name="point">The point in virtual coordinates.</param>
        /// <returns>The point in screen coordinates.</returns>
        public Point PointToScreen(Point point)
        {
            return PointToScreen(point.X, point.Y);
        }

        /// <summary>
        /// Converts a point from virtual coordinates to screen coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate in virtual space.</param>
        /// <param name="y">The y-coordinate in virtual space.</param>
        /// <returns>The point in screen coordinates.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the scale matrix cannot be inverted.
        /// </exception>
        public virtual Point PointToScreen(int x, int y)
        {
            var scaleMatrix = GetScaleMatrix();
            if (!Matrix.Invert(scaleMatrix, out var invertedMatrix))
                throw new InvalidOperationException("Can not project the point, invalid Matrix");
            return (Point)Vector2.Transform(new Vector2(x, y), invertedMatrix);
        }

        /// <summary>
        /// Resets the adapter to its default state.
        /// </summary>
        public virtual void Reset()
        {
        }

        #endregion
    }
}
