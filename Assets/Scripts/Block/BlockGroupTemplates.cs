using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 하드코딩된 블록 그룹 템플릿<br/>
/// LLM으로 수정할 것을 권장함
/// </summary>
public static class BlockGroupTemplates
{
	public static IReadOnlyList<Block.State[,]> Templates => _templates;
	static List<Block.State[,]> _templates = new List<Block.State[,]>()
	{
		 // 1. Single Block (1×1)
		new Block.State[,] {
			{ Block.State.Preview }
		},

		// 2. Domino (2블록: 가로/세로 회전은 동일한 모양)
		new Block.State[,] {
			{ Block.State.Preview, Block.State.Preview }
		},

		// 3. Triomino Line (3블록 직선; 가로/세로 중 하나만)
		new Block.State[,] {
			{ Block.State.Preview, Block.State.Preview, Block.State.Preview }
		},

		// 4. Triomino L-Shape (3블록 L자, 4방향)
		// 원본
		new Block.State[,] {
			{ Block.State.Preview, Block.State.Empty },
			{ Block.State.Preview, Block.State.Preview }
		},
		// 90° 시계 회전
		new Block.State[,] {
			{ Block.State.Preview, Block.State.Preview },
			{ Block.State.Preview, Block.State.Empty }
		},
		// 180° 회전
		new Block.State[,] {
			{ Block.State.Preview, Block.State.Preview },
			{ Block.State.Empty, Block.State.Preview }
		},
		// 270° 회전
		new Block.State[,] {
			{ Block.State.Empty, Block.State.Preview },
			{ Block.State.Preview, Block.State.Preview }
		},

		// 5. 2×2 Square (정사각형; 회전해도 동일)
		new Block.State[,] {
			{ Block.State.Preview, Block.State.Preview },
			{ Block.State.Preview, Block.State.Preview }
		},

		// 6. T Tetromino (4방향)
		// T Up
		new Block.State[,] {
			{ Block.State.Empty,  Block.State.Preview,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview }
		},
		// T Right
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Empty }
		},
		// T Down
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Preview,  Block.State.Empty }
		},
		// T Left
		new Block.State[,] {
			{ Block.State.Empty,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Preview }
		},

		// 7. L Tetromino (왼쪽 상단 돌출, 4방향)
		// 원본
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Empty,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview }
		},
		// 90° 시계 회전
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Empty }
		},
		// 180° 회전
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Empty,  Block.State.Preview }
		},
		// 270° 회전
		new Block.State[,] {
			{ Block.State.Empty,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Preview }
		},

		// 8. Reverse L Tetromino (오른쪽 상단 돌출, 4방향)
		// 원본
		new Block.State[,] {
			{ Block.State.Empty,  Block.State.Empty,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview }
		},
		// 90° 시계 회전
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Preview }
		},
		// 180° 회전
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Empty,  Block.State.Empty }
		},
		// 270° 회전
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Preview }
		},

		// 9. S Tetromino (S자, 2방향)
		// 수평
		new Block.State[,] {
			{ Block.State.Empty,  Block.State.Preview,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Empty }
		},
		// 수직
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Preview }
		},

		// 10. Z Tetromino (Z자, 2방향)
		// 수평
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Empty },
			{ Block.State.Empty,  Block.State.Preview,  Block.State.Preview }
		},
		// 수직
		new Block.State[,] {
			{ Block.State.Empty,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Empty }
		},

		// 11. Plus Shape (십자, 회전해도 동일)
		new Block.State[,] {
			{ Block.State.Empty,  Block.State.Preview,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Preview,  Block.State.Empty }
		},

		// 12. Extended L Shape (위2, 중1, 아래2, 4방향)
		// 원본 (3×2)
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Empty },
			{ Block.State.Preview,  Block.State.Preview }
		},
		// 90° 시계 회전 (2×3)
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Empty,  Block.State.Preview }
		},
		// 180° 회전 (3×2)
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview },
			{ Block.State.Empty,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Preview }
		},
		// 270° 시계 회전 (2×3)
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Empty,  Block.State.Preview },
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview }
		},

		// 13. I Tetromino (4블록 직선; 회전은 동일한 모양)
		new Block.State[,] {
			{ Block.State.Preview,  Block.State.Preview,  Block.State.Preview,  Block.State.Preview }
		}
	};
}
