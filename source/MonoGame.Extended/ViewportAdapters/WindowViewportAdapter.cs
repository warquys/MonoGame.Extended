using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.ViewportAdapters
{
    /// <summary>
    /// A viewport adapter that uses a <see cref="GameWindow"/>'s client bounds as both
    /// the viewport and virtual dimensions. The scale matrix is always identity,
    /// meaning virtual and screen coordinates are 1:1.
    /// </summary>
    public class WindowViewportAdapter : ViewportAdapter
    {
        /// <summary>
        /// The game window used to determine client bounds.
        /// </summary>
        protected readonly GameWindow Window;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowViewportAdapter"/> class.
        /// </summary>
        /// <param name="window">The game window providing the client bounds.</param>
        /// <param name="graphicsDevice">The graphics device used for rendering.</param>
        public WindowViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            Window = window;
            window.ClientSizeChanged += OnClientSizeChanged;
        }

        /// <inheritdoc/>
        public override int ViewportWidth => Window.ClientBounds.Width;

        /// <inheritdoc/>
        public override int ViewportHeight => Window.ClientBounds.Height;

        /// <inheritdoc/>
        public override int VirtualWidth => Window.ClientBounds.Width;

        /// <inheritdoc/>
        public override int VirtualHeight => Window.ClientBounds.Height;

        /// <inheritdoc/>
        public override Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }

        /// <summary>
        /// Handles the window's client size change event by updating the graphics device viewport.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">Event arguments (unused).</param>
        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var x = Window.ClientBounds.Width;
            var y = Window.ClientBounds.Height;

            GraphicsDevice.Viewport = new Viewport(0, 0, x, y);
        }
    }
}
