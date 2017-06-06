﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class QuestNameTable
	{
		public static Dictionary<int, string> IdNameTable = new Dictionary<int, string>
		{
			// Sortie
			{101, "A1"},
			{102, "A2"},
			{103, "A3"},
			{104, "A4"},
			{105, "A5"},
			{106, "A7"},
			{107, "A12"},
			{108, "A8"},
			{109, "A14"},
			{110, "A15"},
			{111, "A17"},
			{112, "A18"},
			{113, "A13"},
			{114, "A19"},
			{115, "A6"},
			{116, "A9"},
			{117, "A11"},
			{118, "A16"},
			{119, "A20"},
			{120, "A10"},
			{121, "A21"},
			{122, "A22"},
			{123, "A23"},
			{124, "A24"},
			{125, "A25"},
			{126, "A26"},
			{127, "A27"},
			{128, "A28"},
			{129, "A29"},
			{130, "A30"},
			{131, "A31"},
			{132, "A32"},
			{133, "A33"},
			{136, "A34"},
			{137, "A35"},
			{138, "A36"},
			{139, "A37"},
			{140, "A38"},
			{141, "A39"},
			{142, "A40"},
			{143, "A41"},
			{144, "A42"},
			{145, "A43"},
			{146, "A45"},
			{147, "A44"},
			{148, "A46"},
			{149, "A47"},
			{150, "A48"},
			{151, "A49"},
			{152, "A50"},
			{153, "A51"},
			{154, "A52"},
			{155, "A53"},
			{156, "A54"},
			{157, "A55"},
			{161, "A56"},
			{162, "A57"},
			{163, "A58"},
			{164, "A59"},
			{165, "A60"},
			{166, "A61"},
			{167, "A62"},
			{168, "A63"},
			{169, "A64"},
			{170, "A65"},
			{171, "A66"},
			{172, "A67"},
			{173, "A68"},
			{174, "A69"},
			{175, "A70"},
			{176, "A71"},
			{177, "A72"},

			// Sortie (Marriage)
			{134, "WA01"},
			{135, "WA02"},

			// Battle
			{202, "B1"},
			{203, "B2"},
			{204, "B4"},
			{205, "B5"},
			{206, "B6"},
			{207, "B7"},
			{208, "B8"},
			{209, "B9"},
			{215, "B3"},
			{217, "B10"},
			{219, "B11"},
			{222, "B12"},
			{223, "B13"},
			{224, "B14"},
			{225, "B15"},
			{227, "B16"},
			{231, "B17"},
			{232, "B18"},
			{233, "B19"},
			{239, "B20"},
			{240, "B21"},
			{244, "B22"},
			{247, "B23"},
			{248, "B24"},
			{250, "B25"},
			{251, "B26"},
			{252, "B27"},
			{253, "B28"},
			{254, "B29"},
			{255, "B30"},
			{258, "B31"},
			{260, "B32"},
			{262, "B33"},
			{263, "B34"},
			{267, "B35"},
			{268, "B36"},
			{269, "B37"},
			{270, "B39"},
			{271, "B38"},
			{272, "B40"},
			{273, "B41"},
			{274, "B42"},
			{275, "B43"},
			{276, "B44"},
			{277, "B45"},
			{278, "B46"},
			{279, "B47"},
			{285, "B49"},
			{286, "B48"},
			{287, "B50"},
			{288, "B51"},
			{289, "B52"},
			{293, "B53"},
			{294, "B54"},
			{295, "B55"},
			{296, "B56"},
			{297, "B57"},
			{805, "B58"},
			{806, "B59"},
			{807, "B60"},
			{808, "B61"},
			{809, "B62"},
			{810, "B63"},
			{811, "B64"},
			{812, "B65"},
			{813, "B66"},
			{814, "B68"},
			{815, "B69"},
			{816, "B67"},
			{817, "B70"},
			{818, "B71"},
			{819, "B72"},
			{820, "B73"},
			{821, "B74"},
			{823, "B75"},
			{824, "B76"},
			{825, "B77"},
			{826, "B78"},
			{827, "B79"},
			{828, "B80"},
			{829, "B82"},
			{830, "B81"},

			// Battle (Daily)
			{201, "Bd1"},
			{210, "Bd3"},
			{211, "Bd4"},
			{212, "Bd6"},
			{216, "Bd2"},
			{218, "Bd5"},
			{226, "Bd7"},
			{230, "Bd8"},

			// Battle (Weekly)
			{213, "Bw3"},
			{214, "Bw1"},
			{220, "Bw2"},
			{221, "Bw4"},
			{228, "Bw5"},
			{229, "Bw6"},
			{241, "Bw7"},
			{242, "Bw8"},
			{243, "Bw9"},
			{261, "Bw10"},

			// Battle (Monthly)
			{249, "Bm1"},
			{256, "Bm2"},
			{257, "Bm3"},
			{259, "Bm4"},
			{264, "Bm6"},
			{265, "Bm5"},
			{266, "Bm7"},

			// Battle (Marriage)
			{245, "WB01"},
			{246, "WB02"},

			// Battle (Quarterly)
			{822, "Bq1"},

			// Practice
			{301, "C1"},
			{302, "C4"},
			{303, "C2"},
			{304, "C3"},
			{307, "C5"},
			{308, "C6"},
			{309, "C7"},
			{311, "C8"},
			{312, "C9"},
			{313, "C10"},

			// Practice (Marriage)
			{306, "WC01"},

			// Expedition
			{401, "D1"},
			{402, "D2"},
			{403, "D3"},
			{404, "D4"},
			{405, "D5"},
			{406, "D6"},
			{408, "D7"},
			{409, "D8"},
			{410, "D9"},
			{411, "D11"},
			{412, "D10"},
			{413, "D12"},
			{414, "D13"},
			{415, "D14"},
			{416, "D15"},
			{417, "D16"},
			{418, "D17"},
			{419, "D18"},
			{420, "D19"},
			{422, "D20"},
			{423, "D21"},
			{424, "D22"},

			// Supply
			{501, "E1"},
			{502, "E2"},
			{503, "E3"},
			{504, "E4"},

			// Arsenal
			{601, "F1"},
			{602, "F2"},
			{603, "F3"},
			{604, "F4"},
			{605, "F5"},
			{606, "F6"},
			{607, "F7"},
			{608, "F8"},
			{609, "F9"},
			{610, "F10"},
			{612, "F11"},
			{613, "F12"},
			{614, "F13"},
			{615, "F14"},
			{616, "F15"},
			{617, "F16"},
			{618, "F17"},
			{619, "F18"},
			{622, "F19"},
			{623, "F20"},
			{624, "F21"},
			{625, "F23"},
			{626, "F22"},
			{627, "F24"},
			{628, "F25"},
			{629, "F26"},
			{630, "F28"},
			{631, "F27"},
			{632, "F29"},
			{633, "F30"},
			{634, "F31"},
			{635, "F32"},
			{636, "F33"},
			{637, "F35"},
			{638, "F34"},
			{639, "F36"},
			{641, "F37"},
			{642, "F38"},
			{643, "F39"},
			{644, "F40"},
			{645, "F41"},
			{646, "F42"},
			{647, "F43"},

			// Arsenal (Marriage)
			{611, "WF01"},

			// Arsenal (Limited time)
			{640, "SF"},

			// Modernization, Powerup
			{701, "G1"},
			{702, "G2"},
			{703, "G3"},
			{704, "G4"},

			// No category? (Limited time)
			{234, "OR1"},
			{421, "OR2"},

			// SN Event?
			{280, "SN01"},
			{620, "SN02"},
			{281, "SN03"},
			{158, "SN04"},
			{282, "SN05"},
			{159, "SN06"},
			{310, "SN07"},
			{621, "SN08"},
			{160, "SN09"},
			{284, "SN10"},
			{283, "SN11"},

			// Saury Season
			{290, "SB03"},
			{291, "SB04"},
			{292, "SB05"},
			{831, "SB12"},
			{832, "SB13"},
			{833, "SB14"},

			// Year End, New Year
			{298, "SB06"},
			{299, "SB07"},
			{801, "SB08"},
			{802, "SB09"},
			{803, "SB10"},
			{804, "SB11"},
		};
	}
}
