using BlazorChess.Components.Grid;
using static BlazorChess.Components.Pieces.PieceColours;

namespace BlazorChess.Components.Pieces
{
	public abstract class ChessPiece(PieceColours.PieceColour colour, string pieceType) : IDisposable
	{

		public PieceColour Colour = colour;
		public string PieceType = pieceType;
		public bool HasMoved { get; set; } = false;
		public int PreviousRow { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public int cachedMove = 0;
		public Dictionary<int, List<(int row, int col)>>? CachedRawMoves { get; set; }
		public virtual bool IsValidMove(int endRow, int endCol, ChessGrid board)
		{
			return GetLegalMoves(board).Any(move => move.row == endRow && move.col == endCol);
		}
		public virtual List<(int row, int col)> GetRawMoves(ChessGrid board)
		{
			if (CachedRawMoves == null || !CachedRawMoves.TryGetValue(cachedMove, out List<(int row, int col)>? value))
			{
				CachedRawMoves = [];
				var moves = CalculateRawMoves(board);
				value = moves;
				CachedRawMoves[cachedMove] = value;
			}
			return value;
		}
		public abstract List<(int row, int col)> CalculateRawMoves(ChessGrid board);
		public virtual List<(int row, int col)> GetLegalMoves(ChessGrid board)
		{
			System.Diagnostics.Debug.Print("Finding legal moves");
			List<(int, int)> legalMoves = [];
			if (board == null || board.GetPiece(Row, Column) == null)
				return legalMoves;
			foreach (var (targetRow, targetCol) in CalculateRawMoves(board))
			{
				if(targetRow == Row && targetCol == Column)
					continue;
				if (this is King && Math.Abs(Column - targetCol) == 2)
				{
					int direction = targetCol > Column ? 1 : -1;
					bool canCastle = true;
					ChessGrid tempBoard = board.Clone();
					if (tempBoard.GetPiece(Row, Column) is not King movingKing)
					{
						continue;
					}
					for (int i = 0; i <= 2; i++)
					{
						int col = Column + (i * direction);

						movingKing.Column = col;
						if (movingKing.Colour == PieceColour.White)
							tempBoard.WhiteKing = movingKing;
						else
							tempBoard.BlackKing = movingKing;

						tempBoard.Grid[Row, col] = movingKing;
						tempBoard.LastMovedPiece = this;
						System.Diagnostics.Debug.Print($"Testing Check for temp board at ({Row},{col})");
						if (tempBoard.IsKingInCheck(movingKing.Colour))
						{
							canCastle = false;	
							break;
						}
						tempBoard.Grid[Row, col] = null;
					}
					if(canCastle)
						legalMoves.Add((targetRow, targetCol));

				}
				else
				{

					ChessGrid tempBoard = board.Clone();
					ChessPiece movingPiece = tempBoard.GetPiece(Row, Column)!;
					tempBoard.Grid[Row, Column] = null;
					movingPiece.Row = targetRow;
					movingPiece.Column = targetCol;
					tempBoard.Grid[targetRow, targetCol] = movingPiece;
					if (movingPiece is King king)
					{
						if (king.Colour == PieceColour.White)
							tempBoard.WhiteKing = king;
						else
							tempBoard.BlackKing = king;
					}
					tempBoard.LastMovedPiece = this;
					System.Diagnostics.Debug.Print($"Testing Check for temp board at ({targetRow},{targetCol})");
					if (!tempBoard.IsKingInCheck(movingPiece.Colour))
					{
						legalMoves.Add((targetRow, targetCol));
					}
				}
			}
			return legalMoves;
		}
		protected void AddLineMoves(List<(int row, int col)> moves, ChessGrid board, int rowStep, int colStep, int maxSteps = 8)
		{
			int r = Row + rowStep;
			int c = Column + colStep;
			int steps = 0;

			while (board.IsInBounds(r, c) && steps < maxSteps)
			{
				var piece = board.Grid[r, c];
				if (piece == null)
				{
					moves.Add((r, c));
				}
				else
				{
					if (piece.Colour != this.Colour)
					{
						moves.Add((r, c));//can capture piece
					}
					break; 
				}
				r += rowStep;
				c += colStep;
				steps++;
			}
		}
		public abstract ChessPiece Clone();

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		//private static readonly Dictionary<string, string> PiecesList = new()
		//{
		//	{ "wK", "♔"
		//	},
		//	{ "wQ", "♕"
		//	},
		//	{ "wR", "♖"
		//	},
		//	{ "wB", "♗"
		//	},
		//	{ "wN", "♘"
		//	},
		//	{ "wP", "♙"
		//	},
		//	{ "bK", "♚"
		//	},
		//	{ "bQ", "♛"
		//	},
		//	{ "bR", "♜"
		//	},
		//	{ "bB", "♝"
		//	},
		//	{ "bN", "♞"
		//	},
		//	{ "bP", "♟"
		//	}
		//};

	}
}

