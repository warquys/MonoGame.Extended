using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace

namespace MonoGame.Extended.ViewportAdapters
{
    /// <summary>
    /// Specifies the type of boxing applied to a <see cref="BoxingViewportAdapter"/> to maintain aspect ratio.
    /// </summary>
    public enum BoxingMode
    {
        /// <summary>
        /// No boxing is applied.
        /// </summary>
        None,
        /// <summary>
        /// Letterboxing is applied (black bars top/bottom).
        /// </summary>
        Letterbox,
        /// <summary>
        /// Pillarboxing is applied (black bars left/right).
        /// </summary>
        Pillarbox
    }

    /// <summary>
    /// A viewport adapter that automatically applies letterboxing or pillarboxing to maintain
    /// the virtual aspect ratio while utilizing the entire window area. Supports optional
    /// bleed areas for safe scaling.
    /// </summary>
    public class BoxingViewportAdapter : ScalingViewportAdapter
    {
        private readonly GameWindow _window;
        private readonly GraphicsDevice _graphicsDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxingViewportAdapter"/> class.
        /// </summary>
        /// <param name="window">The game window providing client bounds.</param>
        /// <param name="graphicsDevice">The graphics device used for rendering.</param>
        /// <param name="virtualWidth">The fixed virtual width in pixels.</param>
        /// <param name="virtualHeight">The fixed virtual height in pixels.</param>
        /// <param name="horizontalBleed">The horizontal bleed area (from left and right edges) that can be safely cut off.</param>
        /// <param name="verticalBleed">The vertical bleed area (from top and bottom edges) that can be safely cut off.</param>
        public BoxingViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight, int horizontalBleed = 0, int verticalBleed = 0)
            : base(graphicsDevice, virtualWidth, virtualHeight)
        {
            _window = window;
            _graphicsDevice = graphicsDevice;
            window.ClientSizeChanged += OnClientSizeChanged;
            HorizontalBleed = horizontalBleed;
            VerticalBleed = verticalBleed;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            _window.ClientSizeChanged -= OnClientSizeChanged;
            base.Dispose();
        }

        /// <summary>
        /// Gets the size of horizontal bleed areas (from left and right edges) which can be safely cut off.
        /// </summary>
        public int HorizontalBleed { get; }

        /// <summary>
        /// Gets the size of vertical bleed areas (from top and bottom edges) which can be safely cut off.
        /// </summary>
        public int VerticalBleed { get; }

        /// <summary>
        /// Gets the current boxing mode being applied.
        /// </summary>
        public BoxingMode BoxingMode { get; private set; }

        /// <summary>
        /// Handles window client size changes by recalculating scale and viewport.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">Event arguments (unused).</param>
        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var clientBounds = _window.ClientBounds;

            var worldScaleX = (float)clientBounds.Width / VirtualWidth;
            var worldScaleY = (float)clientBounds.Height / VirtualHeight;

            var safeScaleX = (float)clientBounds.Width / (VirtualWidth - HorizontalBleed);
            var safeScaleY = (float)clientBounds.Height / (VirtualHeight - VerticalBleed);

            var worldScale = MathHelper.Max(worldScaleX, worldScaleY);
            var safeScale = MathHelper.Min(safeScaleX, safeScaleY);
            var scale = MathHelper.Min(worldScale, safeScale);

            var width = (int)(scale * VirtualWidth + 0.5f);
            var height = (int)(scale * VirtualHeight + 0.5f);

            if (height >= clientBounds.Height && width < clientBounds.Width)
                BoxingMode = BoxingMode.Pillarbox;
            else
            {
                if (width >= clientBounds.Height && height <= clientBounds.Height)
                    BoxingMode = BoxingMode.Letterbox;
                else
                    BoxingMode = BoxingMode.None;
            }

            var x = clientBounds.Width / 2 - width / 2;
            var y = clientBounds.Height / 2 - height / 2;
            GraphicsDevice.Viewport = new Viewport(x, y, width, height);
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            OnClientSizeChanged(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public override Point PointToScreen(int x, int y)
        {
            var viewport = GraphicsDevice.Viewport;
            return base.PointToScreen(x - viewport.X, y - viewport.Y);
        }
    }
}
