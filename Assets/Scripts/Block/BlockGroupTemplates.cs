using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 하드코딩된 블록 그룹 템플릿<br/>
/// LLM으로 수정할 것을 권장함
/// </summary>
public static class BlockGroupTemplates
{
	public static IReadOnlyList<BlockType[,]> Templates => _templates;
	static List<BlockType[,]> _templates = new List<BlockType[,]>()
	{
		 // 1. Single Block (1×1)
		new BlockType[,] {
			{ BlockType.Ghost }
		},

		// 2. Domino (2블록: 가로/세로 회전은 동일한 모양)
		new BlockType[,] {
			{ BlockType.Ghost, BlockType.Ghost }
		},

		// 3. Triomino Line (3블록 직선; 가로/세로 중 하나만)
		new BlockType[,] {
			{ BlockType.Ghost, BlockType.Ghost, BlockType.Ghost }
		},

		// 4. Triomino L-Shape (3블록 L자, 4방향)
		// 원본
		new BlockType[,] {
			{ BlockType.Ghost, BlockType.Empty },
			{ BlockType.Ghost, BlockType.Ghost }
		},
		// 90° 시계 회전
		new BlockType[,] {
			{ BlockType.Ghost, BlockType.Ghost },
			{ BlockType.Ghost, BlockType.Empty }
		},
		// 180° 회전
		new BlockType[,] {
			{ BlockType.Ghost, BlockType.Ghost },
			{ BlockType.Empty, BlockType.Ghost }
		},
		// 270° 회전
		new BlockType[,] {
			{ BlockType.Empty, BlockType.Ghost },
			{ BlockType.Ghost, BlockType.Ghost }
		},

		// 5. 2×2 Square (정사각형; 회전해도 동일)
		new BlockType[,] {
			{ BlockType.Ghost, BlockType.Ghost },
			{ BlockType.Ghost, BlockType.Ghost }
		},

		// 6. T Tetromino (4방향)
		// T Up
		new BlockType[,] {
			{ BlockType.Empty,  BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost }
		},
		// T Right
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Empty }
		},
		// T Down
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Ghost,  BlockType.Empty }
		},
		// T Left
		new BlockType[,] {
			{ BlockType.Empty,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Ghost }
		},

		// 7. L Tetromino (왼쪽 상단 돌출, 4방향)
		// 원본
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Empty,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost }
		},
		// 90° 시계 회전
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Empty }
		},
		// 180° 회전
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Empty,  BlockType.Ghost }
		},
		// 270° 회전
		new BlockType[,] {
			{ BlockType.Empty,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Ghost }
		},

		// 8. Reverse L Tetromino (오른쪽 상단 돌출, 4방향)
		// 원본
		new BlockType[,] {
			{ BlockType.Empty,  BlockType.Empty,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost }
		},
		// 90° 시계 회전
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Ghost }
		},
		// 180° 회전
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Empty,  BlockType.Empty }
		},
		// 270° 회전
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Ghost }
		},

		// 9. S Tetromino (S자, 2방향)
		// 수평
		new BlockType[,] {
			{ BlockType.Empty,  BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Empty }
		},
		// 수직
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Ghost }
		},

		// 10. Z Tetromino (Z자, 2방향)
		// 수평
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Empty,  BlockType.Ghost,  BlockType.Ghost }
		},
		// 수직
		new BlockType[,] {
			{ BlockType.Empty,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Empty }
		},

		// 11. Plus Shape (십자, 회전해도 동일)
		new BlockType[,] {
			{ BlockType.Empty,  BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Ghost,  BlockType.Empty }
		},

		// 12. Extended L Shape (위2, 중1, 아래2, 4방향)
		// 원본 (3×2)
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Empty },
			{ BlockType.Ghost,  BlockType.Ghost }
		},
		// 90° 시계 회전 (2×3)
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Empty,  BlockType.Ghost }
		},
		// 180° 회전 (3×2)
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost },
			{ BlockType.Empty,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Ghost }
		},
		// 270° 시계 회전 (2×3)
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Empty,  BlockType.Ghost },
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost }
		},

		// 13. I Tetromino (4블록 직선; 회전은 동일한 모양)
		new BlockType[,] {
			{ BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost,  BlockType.Ghost }
		}
	};
}
