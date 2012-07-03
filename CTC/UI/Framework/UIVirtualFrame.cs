﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CTC
{
    public class UIVirtualFrame : UIFrame
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public UIScrollbar Scrollbar;

        /// <summary>
        /// The size and position of the scroll area inside of this view.
        /// </summary>
        public Rectangle VirtualBounds
        {
            get
            {
                return _VirtualBounds;
            }
            set
            {
                ContentView.Bounds.Y = ClientBounds.Top - value.Y;
                _VirtualBounds = value;
            }
        }
        private Rectangle _VirtualBounds = new Rectangle(0, 0, 0, 0);

        #endregion

        public UIVirtualFrame()
        {
            Scrollbar = (UIScrollbar)AddSubview(new UIScrollbar());
            ClipsSubviews = true;
        }

        #region Property overrides
        
        public override Rectangle ClientBounds
        {
            get
            {
                Rectangle b = base.ClientBounds;
                b.Width -= Scrollbar.Bounds.Width;
                return b;
            }
        }

        #endregion

        public override void LayoutSubviews()
        {
            // Position the scrollbar to the right
            Margin sp = SkinPadding;
            Rectangle sc = sp.SubtractFrom(Bounds);
            Scrollbar.Bounds = new Rectangle
            {
                X = Bounds.Width - sp.Right - Scrollbar.Bounds.Width,
                Y = sp.Top,
                Width = Scrollbar.Bounds.Width,
                Height = Bounds.Height - sp.TotalHeight
            };

            // Set the scrollbar to something sane
            // Important we call this before base.Layout, since
            // the scrollbar will make use of the position to position the gem.
            if (ContentView.FullBounds.Height > ClientBounds.Height)
                Scrollbar.ScrollbarLength = ContentView.FullBounds.Height;
            else
                Scrollbar.ScrollbarLength = 0;

            // This will layout the buttons etc. on the frame
            base.LayoutSubviews();

            // Now we layout the content view as *we* want it.
            if (ContentView.FullBounds.Height > ClientBounds.Height)
            {
                // Content is larger than we are...
                VirtualBounds = new Rectangle(
                    0, 0,
                    ClientBounds.Width,
                    ContentView.FullBounds.Height
                );
            }
        }

        protected override void DrawChildren(SpriteBatch CurrentBatch, Rectangle BoundingBox)
        {
            base.DrawChildren(CurrentBatch, new Rectangle(VirtualBounds.X, VirtualBounds.Y, Bounds.Width, Bounds.Height));
        }
    }
}
