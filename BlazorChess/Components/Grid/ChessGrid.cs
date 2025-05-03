using BlazorChess.Components.Pieces;
using System.Data.Common;
using System.Net.NetworkInformation;
using static BlazorChess.Components.Pieces.PieceColours;

namespace BlazorChess.Components.Grid
{
	public partial class ChessGrid
	{
		private int _width = 8;
		private int _height = 8;

		public int Width
		{
			get => _width;
			set => _width = value;
		}

		public int Height
		{
			get => _height;
			set => _height = value;
		}
		public List<MoveRecord> MoveHistory { get; set; } = [];

		public King? WhiteKing { get; set; } = null;
		public King? BlackKing { get; set; } = null;
		public int CurrentMoveIndex { get; set; } = 0;
		public ChessPiece? LastMovedPiece { get; set; } = null;
		public PieceColour ColourToMove => LastMovedPiece == null || LastMovedPiece.Colour == PieceColour.Black ? PieceColour.White : PieceColour.Black;
		public ChessPiece?[,] Grid { get; set; }

		public ChessGrid()
		{
			Grid = new ChessPiece?[Width, Height];
		}
		public void InitializeBoard()
		{
			foreach (ChessPiece? piece in Grid)
			{
				if (piece != null)
				{
					Grid[piece.Row, piece.Column] = null;
					piece.Dispose();
				}
			}
			for (int i = 0; i < Width; i++)
			{

				Grid[1, i] = new Pawn(PieceColour.White) { Row = 1, Column = i };
				Grid[6, i] = new Pawn(PieceColour.Black) { Row = 6, Column = i };
			}
			Grid[0, 0] = new Rook(PieceColour.White) { Row = 0, Column = 0 };
			Grid[0, 1] = new Knight(PieceColour.White) { Row = 0, Column = 1 };
			Grid[0, 2] = new Bishop(PieceColour.White) { Row = 0, Column = 2 };
			Grid[0, 3] = new Queen(PieceColour.White) { Row = 0, Column = 3 };
			WhiteKing = new King(PieceColour.White) { Row = 0, Column = 4 };
			Grid[0, 4] = WhiteKing;
			Grid[0, 5] = new Bishop(PieceColour.White) { Row = 0, Column = 5 };
			Grid[0, 6] = new Knight(PieceColour.White) { Row = 0, Column = 6 };
			Grid[0, 7] = new Rook(PieceColour.White) { Row = 0, Column = 7 };
			Grid[7, 0] = new Rook(PieceColour.Black) { Row = 7, Column = 0 };
			Grid[7, 1] = new Knight(PieceColour.Black) { Row = 7, Column = 1 };
			Grid[7, 2] = new Bishop(PieceColour.Black) { Row = 7, Column = 2 };
			Grid[7, 3] = new Queen(PieceColour.Black) { Row = 7, Column = 3 };
			BlackKing = new King(PieceColour.Black) { Row = 7, Column = 4 };
			Grid[7, 4] = BlackKing;
			Grid[7, 5] = new Bishop(PieceColour.Black) { Row = 7, Column = 5 };
			Grid[7, 6] = new Knight(PieceColour.Black) { Row = 7, Column = 6 };
			Grid[7, 7] = new Rook(PieceColour.Black) { Row = 7, Column = 7 };
		}
		public string GetPieceSymbol(int x, int y)
		{
			if (Grid[x, y] != null)
			{
				return Grid[x, y]!.PieceType;
			}
			return string.Empty;
		}
		public ChessPiece? GetPiece(int x, int y)
		{
			if(x >= 0 && x < Height && y >= 0 && y < Width)
			{
				return Grid[x, y];
			}
			return null;
		}
		public bool CheckPieceMove(ChessPiece selectedPiece, int endRow, int endCol)
		{
			if (selectedPiece == null)
			{
				return false;
			}
			if (selectedPiece.Colour != ColourToMove)
			{
				return false;
			}
			//check for additional castling user input variations
			if (selectedPiece is Rook && GetPiece(endRow, endCol) is King tempKing)
			{
				if (selectedPiece.Colour == tempKing.Colour)
				{
					endCol = selectedPiece.Column;
					selectedPiece = tempKing;
				}
			}
			int direction = Math.Sign(endCol - selectedPiece.Column);

			if (selectedPiece is King && Math.Abs(endCol - selectedPiece.Column) >= 2)
			{
				endCol = selectedPiece.Column + direction * 2;
			}
			System.Diagnostics.Debug.Print("Check if valid move");

			if (selectedPiece.IsValidMove(endRow, endCol, this))
			{
				if (selectedPiece is King king && Math.Abs(endCol - selectedPiece.Column) >= 2)
				{
					int rookCol = direction == 1 ? 7 : 0;
					if (Grid[selectedPiece.Row, rookCol] is Rook rook)
					{
						AddMoveRecord(MovePieceCastle(king, rook, selectedPiece.Column + 2 * direction, selectedPiece.Column + direction));
						return true;
					}
				}
				//other
				else
				{
					ChessPiece? capturedPiece = Grid[endRow, endCol];
					if (selectedPiece is Pawn pawn)//check en passant, used to store captured piece correctly
					{
						if (selectedPiece.Column != endCol && GetPiece(endRow, endCol) == null)
						{
							int capturedPawnRow = pawn.Colour == PieceColour.White ? endRow - 1 : endRow + 1;
							capturedPiece = Grid[capturedPawnRow, endCol];
							Grid[capturedPawnRow, endCol] = null;
						}
					}
					AddMoveRecord(MovePiece(selectedPiece, endRow, endCol, capturedPiece));
					return true;
				}
			}
			return false;
		}
		public MoveRecord MovePiece(ChessPiece selectedPiece, int endRow, int endCol, ChessPiece? capturedPiece)
		{
			int startRow = selectedPiece.Row;
			int	startCol =  selectedPiece.Column;
			Grid[startRow, startCol] = null;
			Grid[endRow, endCol] = selectedPiece;
			selectedPiece.PreviousRow = startRow;
			selectedPiece.Row = endRow;
			selectedPiece.Column = endCol;
			selectedPiece.HasMoved = true;
			LastMovedPiece = selectedPiece;
			if (selectedPiece is King king )
			{
				if(king.Colour == PieceColour.White)
					WhiteKing = king;
				else
					BlackKing = king;
			}
			return new MoveRecord(selectedPiece, startRow, startCol, endRow, endCol, capturedPiece, (MoveHistory.Count / 2) + 1, this.Clone());
		}
		public MoveRecord MovePieceCastle(King king, Rook rook, int kingEndCol, int rookEndCol)
		{
			int kingStartCol = king.Column;
			Grid[king.Row, king.Column] = null;
			Grid[king.Row, kingEndCol] = king;
			Grid[rook.Row, rook.Column] = null;
			Grid[rook.Row, rookEndCol] = rook;
			king.Column = kingEndCol;
			rook.Column = rookEndCol;
			king.HasMoved = true;
			rook.HasMoved = true;
			LastMovedPiece = king;

			if(king.Colour == PieceColour.White)
				WhiteKing = king;
			else
				BlackKing = king;

			return new MoveRecord(king, king.Row, kingStartCol, king.Row, kingEndCol, null, (MoveHistory.Count / 2) + 1, this.Clone());
			
		}
		//public void PromotePawn(Pawn pawn, ChessPiece chosenPiece)
		//{

		//}

		public void AddMoveRecord(MoveRecord move)
		{
			if (CurrentMoveIndex < MoveHistory.Count)
			{
				MoveHistory.RemoveRange(CurrentMoveIndex, MoveHistory.Count - CurrentMoveIndex);
				MoveHistory.Add(move);
				CurrentMoveIndex = MoveHistory.Count;
			}
			else
			{
				MoveHistory.Add(move);
				CurrentMoveIndex = MoveHistory.Count;
			}
		}
		public ChessGrid Clone()
		{
			ChessGrid clone = new()
			{
				Width = Width,
				Height = Height,
				LastMovedPiece = LastMovedPiece?.Clone(),
				WhiteKing = WhiteKing?.Clone() as King,
				BlackKing = BlackKing?.Clone() as King,
				Grid = new ChessPiece?[Width, Height],
				CurrentMoveIndex = CurrentMoveIndex

			};
			if (Grid != null)
			{
				// clone each piece
				for (int row = 0; row < Width; row++)
				{
					for (int col = 0; col < Height; col++)
					{
						if (Grid[row, col] != null)
						{
							clone.Grid[row, col] = Grid[row, col]!.Clone(); 
						}
					}
				}
			}
			return clone;
		}
		public bool IsKingInCheck(PieceColour colour)
		{
			King? king = colour == PieceColour.White ? WhiteKing : BlackKing;
			if (king == null)
			{
				return false;
			}
			System.Diagnostics.Debug.Print("START OF CALC LOOP");
			for (int row = 0; row < Height; row++)
			{
				for (int col = 0; col < Width; col++)
				{
					ChessPiece? piece = Grid[row, col];
					if (piece != null && piece.Colour != colour && piece is not King)
					{
						System.Diagnostics.Debug.Print($"Piece colour :{piece.Colour}, Piece Type :{piece.PieceType} calcing raw moves");
						if (piece.CalculateRawMoves(this).Any(move => move.row == king.Row && move.col == king.Column))
						{
							return true;
						}
					}
				}
			}
			System.Diagnostics.Debug.Print("END OF CALC LOOP");

			return false;
		}

		public void RestoreBoardToMove(int index)
		{
			if (index == 0)
			{
				InitializeBoard();
				CurrentMoveIndex = 0;
			}
			else if (index <= MoveHistory.Count)
			{
				var moveHist = MoveHistory;

				ChessGrid snapshot = MoveHistory[index-1].BoardSnapshot.Clone();

				this.Width = snapshot.Width;
				this.Height = snapshot.Height;
				this.LastMovedPiece = snapshot.LastMovedPiece?.Clone();
				this.WhiteKing = snapshot.WhiteKing?.Clone() as King;
				this.BlackKing = snapshot.BlackKing?.Clone() as King;
				this.Grid = new ChessPiece?[Width, Height];
				for (int row = 0; row < Width; row++)
				{
					for (int col = 0; col < Height; col++)
					{
						this.Grid[row, col] = snapshot.Grid[row, col]?.Clone();
					}
				}
				this.MoveHistory = moveHist;
				CurrentMoveIndex = index;
			}
		}
		public bool IsInBounds(int row, int col)
		{
			if (row < Height && row >= 0 && col < Width && col >= 0)
				return true;
			return false;
		}
	}
}
