using System;
using NStack;
using Terminal.Gui;

namespace ConsoleApp.TerminalGuiPlus
{
    public class MyButton : View {
		ustring? text;
		bool is_default;
		
		/// <summary>
		///   Initializes a new instance of <see cref="Button"/> using <see cref="LayoutStyle.Computed"/> layout.
		/// </summary>
		/// <remarks>
		///   The width of the <see cref="Button"/> is computed based on the
		///   text length. The height will always be 1.
		/// </remarks>
		/// <param name="text">The button's text</param>
		/// <param name="is_default">
		///   If <c>true</c>, a special decoration is used, and the user pressing the enter key 
		///   in a <see cref="Dialog"/> will implicitly activate this button.
		/// </param>
		public MyButton (ustring text, bool is_default = false)
		{
			Init (text, is_default);
		}

		/// <summary>
		///   Initializes a new instance of <see cref="Button"/> using <see cref="LayoutStyle.Absolute"/> layout, based on the given text
		/// </summary>
		/// <remarks>
		///   The width of the <see cref="Button"/> is computed based on the
		///   text length. The height will always be 1.
		/// </remarks>
		/// <param name="x">X position where the button will be shown.</param>
		/// <param name="y">Y position where the button will be shown.</param>
		/// <param name="text">The button's text</param>
		public MyButton (int x, int y, ustring text) : this (x, y, text, false) { }

		/// <summary>
		///   Initializes a new instance of <see cref="Button"/> using <see cref="LayoutStyle.Absolute"/> layout, based on the given text.
		/// </summary>
		/// <remarks>
		///   The width of the <see cref="Button"/> is computed based on the
		///   text length. The height will always be 1.
		/// </remarks>
		/// <param name="x">X position where the button will be shown.</param>
		/// <param name="y">Y position where the button will be shown.</param>
		/// <param name="text">The button's text</param>
		/// <param name="is_default">
		///   If <c>true</c>, a special decoration is used, and the user pressing the enter key 
		///   in a <see cref="Dialog"/> will implicitly activate this button.
		/// </param>
		public MyButton (int x, int y, ustring text, bool is_default)
		    : base (new Rect (x, y, text.RuneCount + 4 + (is_default ? 2 : 0), 1))
		{
			Init (text, is_default);
		}

		Rune _leftBracket;
		Rune _rightBracket;
		Rune _leftDefault;
		Rune _rightDefault;

		void Init (ustring text, bool is_default)
		{
			HotKeySpecifier = new Rune ('_');

			_leftBracket = new Rune (Driver != null ? Driver.LeftBracket : '[');
			_rightBracket = new Rune (Driver != null ? Driver.RightBracket : ']');
			_leftDefault = new Rune (Driver != null ? Driver.LeftDefaultIndicator : '<');
			_rightDefault = new Rune (Driver != null ? Driver.RightDefaultIndicator : '>');

			CanFocus = true;
			this.IsDefault = is_default;
			Text = text ?? string.Empty;
		}

		/// <summary>
		///   The text displayed by this <see cref="Button"/>.
		/// </summary>
		public new ustring Text {
			get {
				if (text == null) { throw new Exception("Value was null"); }
				return text;
			}

			set {
				text = value;
				Update ();
			}
		}

		public void setText(String text)
		{
			this.text = text;
		}

		/// <summary>
		/// Gets or sets whether the <see cref="Button"/> is the default action to activate in a dialog.
		/// </summary>
		/// <value><c>true</c> if is default; otherwise, <c>false</c>.</value>
		public bool IsDefault {
			get => is_default;
			set {
				is_default = value;
				Update ();
			}
		}
		
		internal void Update ()
		{
			if (IsDefault)
				base.Text = ustring.Make(_leftBracket) + ustring.Make(_leftDefault) + " " + text + " " +
				            ustring.Make(_rightDefault) + ustring.Make(_rightBracket);
			else
				base.Text = ustring.Make("") + text;

			int w = base.Text.RuneCount - (base.Text.Contains (HotKeySpecifier) ? 1 : 0);
			Width = w;
			Height = 1;
			Frame = new Rect (Frame.Location, new Size (w, 1));
			SetNeedsDisplay ();
		}

		bool CheckKey (KeyEvent key)
		{
			if (key.Key == (Key.AltMask | HotKey)) {
				SetFocus ();
				Clicked?.Invoke ();
				return true;
			}
			return false;
		}

		///<inheritdoc/>
		public override bool ProcessHotKey (KeyEvent kb)
		{
			if (kb.IsAlt)
				return CheckKey (kb);

			return false;
		}

		///<inheritdoc/>
		public override bool ProcessColdKey (KeyEvent kb)
		{
			if (IsDefault && kb.KeyValue == '\n') {
				Clicked?.Invoke ();
				return true;
			}
			return CheckKey (kb);
		}

		///<inheritdoc/>
		public override bool ProcessKey (KeyEvent kb)
		{
			var c = kb.KeyValue;
			if (c == '\n' || c == ' ' || kb.Key == HotKey) {
				Clicked?.Invoke ();
				return true;
			}
			return base.ProcessKey (kb);
		}


		/// <summary>
		///   Clicked <see cref="Action"/>, raised when the user clicks the primary mouse button within the Bounds of this <see cref="View"/>
		///   or if the user presses the action key while this view is focused. (TODO: IsDefault)
		/// </summary>
		/// <remarks>
		///   Client code can hook up to this event, it is
		///   raised when the button is activated either with
		///   the mouse or the keyboard.
		/// </remarks>
		public Action? Clicked;

		///<inheritdoc/>
		public override bool MouseEvent (MouseEvent me)
		{
			if (me.Flags == MouseFlags.Button1Clicked || me.Flags == MouseFlags.Button1DoubleClicked ||
				me.Flags == MouseFlags.Button1TripleClicked) {
				if (CanFocus) {
					if (!HasFocus) {
						SetFocus ();
						SetNeedsDisplay ();
					}
					Clicked?.Invoke ();
				}

				return true;
			}
			return false;
		}

		///<inheritdoc/>
		public override void PositionCursor ()
		{
			if (HotKey == Key.Unknown) {
				for (int i = 0; i < base.Text.RuneCount; i++) {
					if (base.Text [i] == text! [0]) {
						Move (i, 0);
						return;
					}
				}
			}
			base.PositionCursor ();
		}
	}
}