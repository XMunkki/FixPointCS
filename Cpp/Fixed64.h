//
// GENERATED FILE!!!
//
// Generated from Fixed64.cs, part of the FixPointCS project (MIT license).
//
#pragma once
#ifndef __FIXED64_H
#define __FIXED64_H

// Include 64bit numeric types
#include <stdint.h>

// If FP_ASSERT is not custom-defined, then use the standard one
#ifndef FP_ASSERT
#include <assert.h>
#endif

namespace Fixed64
{
    typedef int FP_INT;
    typedef unsigned int FP_UINT;
    typedef int64_t FP_LONG;
    typedef uint64_t FP_ULONG;

    static_assert(sizeof(FP_INT) == 4, "Wrong bytesize for FP_INT");
    static_assert(sizeof(FP_UINT) == 4, "Wrong bytesize for FP_UINT");
    static_assert(sizeof(FP_LONG) == 8, "Wrong bytesize for FP_LONG");
    static_assert(sizeof(FP_ULONG) == 8, "Wrong bytesize for FP_ULONG");

    #ifndef FP_ASSERT
    #define FP_ASSERT(x) assert(x)
    #endif

    static const double FP_PI = 3.14159265359;

//
// FixPointCS
//
// Copyright(c) 2018 Jere Sanisalo, Petri Kero
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY INT64_C(C)AIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER INT64_C(DEA)INGS IN THE
// SOFTWARE.
//

    static FP_INT Qmul29(FP_INT a, FP_INT b)
    {
        return (FP_INT)((FP_LONG)a * (FP_LONG)b >> 29);
    }

    static FP_INT Qmul30(FP_INT a, FP_INT b)
    {
        return (FP_INT)((FP_LONG)a * (FP_LONG)b >> 30);
    }

    // Exp2()

	// Precision: 13.24 bits
	static FP_INT Exp2Poly3(FP_INT a)
	{
		FP_INT y = Qmul30(a, 84039593); // 0.0782679701835315868647357253725971674790033117245148781445598202137415363194904317749528903660739148499430948967629357887
		y = Qmul30(a, y + 242996024); // 0.226307682289372255347421644246257966273699535419878898050811760122384683941875786929647503217974831952486347597791720611
		y = Qmul30(a, y + 746706207); // 0.695424347527096157787842630381144866247297152855606223804628419663873779738633781295399606415951253197570557505445343601
		y = y + 1073741824;
		return y;
	}

	// Precision: 18.19 bits
	static FP_INT Exp2Poly4(FP_INT a)
	{
		FP_INT y = Qmul30(a, 14555373); // 0.0135557472348149177040307931905578544538124307723745221579881209426474911809748672636364432116420009120178935332926148611
		y = Qmul30(a, y + 55869331); // 0.0520323690084328924674487312215472415900450170687696511359785661622616863440911035364584944748959228308174520922142865995
		y = Qmul30(a, y + 259179547); // 0.241379762937091639018661074809242143033914474070411268883151785382581196588939527676019951096295687987322799846420118765
		y = Qmul30(a, y + 744137573); // 0.693032120819660550809859400778652760922228078088444557822881527512509625885994501523885111217166388269841854528072979774
		y = y + 1073741824;
		return y;
	}

	// Precision: 23.37 bits
	static FP_INT Exp2Poly5(FP_INT a)
	{
		FP_INT y = Qmul30(a, 2017903); // 0.00187931864849444079178064366523643962831734445578833344828943266930262096728457318136293441770024748382988959051143223706
		y = Qmul30(a, y + 9654007); // 0.0089909950956369787948425038952611903126353369666002380364841111113291819538448433335270460143993823536893996134420311419
		y = Qmul30(a, y + 59934847); // 0.0558186759615980431203104787377782200574008231751237416643519892213181468390965300572726745159591410385167709971364406724
		y = Qmul30(a, y + 257869054); // 0.240159271464382269128561549965297376067896183861587452990582800168980375711748974675500420263841816476095364031528449763
		y = Qmul30(a, y + 744266012); // 0.693151738829888268164504823736426773933750311540900233860291666829069674528025078752336924788099412647868575767381646186
		y = y + 1073741824;
		return y;
	}

    // Rcp()

	// Precision: 11.33 bits
	static FP_INT RcpPoly4(FP_INT a)
	{
		FP_INT y = Qmul30(a, 166123244); // 0.154714327545457094588979713106287560782537959277436051827019427328357322113481152734370734443893548182187731839301875899
		y = Qmul30(a, y + -581431354); // -0.54150014640909983106142899587200646273888285747102618139456799564925062739718403457029757055362741863765712410515426083
		y = Qmul30(a, y + 939345296); // 0.874833479742433164394762329205339796072216190804359514727901328982583960730517367903630903886960751970990489874952162084
		y = Qmul30(a, y + -1060908097); // -0.988047660878790427922313046439620894115871292610769385160352760661690655446814486067704067777226881515521097609099777152
		y = y + 1073741824;
		return y;
	}

	// Precision: 16.53 bits
	static FP_INT RcpPoly6(FP_INT a)
	{
		FP_INT y = Qmul30(a, 77852993); // 0.0725062501842326696626758301282171253618850679805450684783331254738896577827939599454470990870969993306249485759929666981
		y = Qmul30(a, y + -350338469); // -0.326278125829047013482041235576977064128482805912452808152499064632503460022572819754511945891936496987812268591968349959
		y = Qmul30(a, y + 723231606); // 0.673561921455982545223734340382301840739167811454662043292934558465813280438563313561682188125190962075365760110255605888
		y = Qmul30(a, y + -974250754); // -0.907341721411285515029553588773713935349385980480415781958608445152553325879092398556945676696228380651281188866035287881
		y = Qmul30(a, y + 1059679220); // 0.986903179099804504543521516658287329916121575881841375617775839327272856972646872081214163224722216327427202183393238047
		y = Qmul30(a, y + -1073045505); // -0.999351503499687190918336862818115296539305668924179897277936013481919009292338927276885827848845300094324453411638172793
		y = y + 1073741824;
		return y;
	}

	static FP_INT RcpPoly3Lut4Table[] =
	{
		-678697788, 1018046684, -1071069948, 1073721112,
		-302893157, 757232894, -1008066289, 1068408287,
		-154903745, 542163110, -902798392, 1051046118,
		-87262610, 392681750, -792180891, 1023631333,
	};

	// Precision: 15.66 bits
	static FP_INT RcpPoly3Lut4(FP_INT a)
	{
		FP_INT offset = (a >> 28) * 4;
		FP_INT y = Qmul30(a, RcpPoly3Lut4Table[offset + 0]);
		y = Qmul30(a, y + RcpPoly3Lut4Table[offset + 1]);
		y = Qmul30(a, y + RcpPoly3Lut4Table[offset + 2]);
		y = y + RcpPoly3Lut4Table[offset + 3];
		return y;
	}

	static FP_INT RcpPoly4Lut8Table[] =
	{
		796773553, -1045765287, 1072588028, -1073726795, 1073741824,
		456453183, -884378041, 1042385791, -1071088216, 1073651788,
		276544830, -708646126, 977216564, -1060211779, 1072962711,
		175386455, -559044324, 893798171, -1039424537, 1071009496,
		115547530, -440524957, 805500803, -1010097984, 1067345574,
		78614874, -348853503, 720007233, -974591889, 1061804940,
		54982413, -278348465, 641021491, -935211003, 1054431901,
		39383664, -223994590, 569927473, -893840914, 1045395281,
	};

	// Precision: 24.07 bits
	static FP_INT RcpPoly4Lut8(FP_INT a)
	{
		FP_INT offset = (a >> 27) * 5;
		FP_INT y = Qmul30(a, RcpPoly4Lut8Table[offset + 0]);
		y = Qmul30(a, y + RcpPoly4Lut8Table[offset + 1]);
		y = Qmul30(a, y + RcpPoly4Lut8Table[offset + 2]);
		y = Qmul30(a, y + RcpPoly4Lut8Table[offset + 3]);
		y = y + RcpPoly4Lut8Table[offset + 4];
		return y;
	}

    // Sqrt()

	// Precision: 13.36 bits
	static FP_INT SqrtPoly3(FP_INT a)
	{
		FP_INT y = Qmul30(a, 26809804); // 0.0249685755493961204934845015323729712245958715357182065425848552518546416164312449413742280712638308483065114885417147904
		y = Qmul30(a, y + -116435772); // -0.108439263715492087333244576730247754908569708153374339944951137491994192013534152641012071161185446185655458733810736431
		y = Qmul30(a, y + 534384395); // 0.497684250539191015641448799407572862253645711994604206579046020230872028859209946550025377417563188072362793476181318665
		y = y + 1073741824; // 1.0
		return y;
	}

	// Precision: 16.50 bits
	static FP_INT SqrtPoly4(FP_INT a)
	{
		FP_INT y = Qmul30(a, -11559524); // -0.0107656468280005064933278905326776959702034851444407595549875999349858889266381514341825269487372902092181743561671344361
		y = Qmul30(a, y + 49235626); // 0.0458542501550120083313075597659725264999808459122954966477604412728019257521420334516113399358029950852981420572751187192
		y = Qmul30(a, y + -129356986); // -0.120473082434524586846319215086079446047783981719931015048359535322204769538816469963000080874223090148906445903268633479
		y = Qmul30(a, y + 536439312); // 0.499598041480608133810028270062482694087678496329024351132266431975121211175419626795958802214798958007840324433072946221
		y = y + 1073741824; // 1.0
		return y;
	}

	static FP_INT SqrtPoly3Lut8Table[] =
	{
		57835763, -133550637, 536857054, 1073741824,
		43771091, -128445855, 536217068, 1073769530,
		34067722, -121273511, 534434402, 1073918540,
		27129178, -113536005, 531547139, 1074279077,
		22019236, -105917226, 527752485, 1074910452,
		18161894, -98716852, 523266057, 1075843557,
		15188335, -92049348, 518277843, 1077088717,
		12854281, -85939307, 512942507, 1078642770,
	};

	// Precision: 23.56 bits
	static FP_INT SqrtPoly3Lut8(FP_INT a)
	{
		FP_INT offset = (a >> 27) * 4;
		FP_INT y = Qmul30(a, SqrtPoly3Lut8Table[offset + 0]);
		y = Qmul30(a, y + SqrtPoly3Lut8Table[offset + 1]);
		y = Qmul30(a, y + SqrtPoly3Lut8Table[offset + 2]);
		y = y + SqrtPoly3Lut8Table[offset + 3];
		return y;
	}

    // RSqrt()

	// Precision: 10.55 bits
	static FP_INT RSqrtPoly3(FP_INT a)
	{
		FP_INT y = Qmul30(a, -91950555); // -0.0856356289309618075724442347978716997984112060739604608172096078728382955692378474864988406402256175535135909431476122756
		y = Qmul30(a, y + 299398639); // 0.278836710932968623313626076681628936089988230155462820435332822241435754263689225928134347217644388668377307808008581711
		y = Qmul30(a, y + -521939780); // -0.486094300815459291340337479778908197006741086393028323029783345373231219463397859016441739413597984747356793749404820923
		y = y + 1073741824; // 1.0
		return y;
	}

	// Precision: 16.08 bits
	static FP_INT RSqrtPoly5(FP_INT a)
	{
		FP_INT y = Qmul30(a, -34036183); // -0.0316986662178132948125724057457789067274319219669948992806572724657733410288354401675056668794389506376695226173434879395
		y = Qmul30(a, y + 140361627); // 0.130721952132469025002475913996909202114937889568059538633961150597311078891698356228999013320515864372663894767082977274
		y = Qmul30(a, y + -276049470); // -0.257091104134768572313864227992129218223743238765168155465701243532365537275047394354439662434976402678179717893043406024
		y = Qmul30(a, y + 391366758); // 0.364488696521754181874287383887070965401384329257252476170774837632778316357675818762347533724407215505228711973415321601
		y = Qmul30(a, y + -536134428); // -0.4993140971150938153494823020412230032803111204046749234700376032365842777144378210442074505666869401945364431146552564
		y = y + 1073741824; // 1.0
		return y;
	}

	static FP_INT RSqrtPoly3Lut16Table[] =
	{
		-301579590, 401404709, -536857690, 1073741824,
		-245423010, 391086820, -536203235, 1073727515,
		-202026137, 374967334, -534189977, 1073642965,
		-168017146, 355951863, -530632261, 1073420226,
		-141028602, 335796841, -525604155, 1073001192,
		-119367482, 315555573, -519290609, 1072343850,
		-101802870, 295846496, -511911750, 1071422108,
		-87426328, 277017299, -503685655, 1070223323,
		-75558212, 259246781, -494811415, 1068745317,
		-65683680, 242608795, -485462769, 1066993613,
		-57408255, 227112748, -475787122, 1064979109,
		-50426484, 212729399, -465907121, 1062716254,
		-44499541, 199407328, -455923331, 1060221646,
		-39439007, 187083448, -445917204, 1057513002,
		-35094980, 175689646, -435953979, 1054608400,
		-31347269, 165156947, -426085312, 1051525761,
	};

	// Precision: 24.59 bits
	static FP_INT RSqrtPoly3Lut16(FP_INT a)
	{
		FP_INT offset = (a >> 26) * 4;
		FP_INT y = Qmul30(a, RSqrtPoly3Lut16Table[offset + 0]);
		y = Qmul30(a, y + RSqrtPoly3Lut16Table[offset + 1]);
		y = Qmul30(a, y + RSqrtPoly3Lut16Table[offset + 2]);
		y = y + RSqrtPoly3Lut16Table[offset + 3];
		return y;
	}

    // Log()

	// Precision: 12.18 bits
	static FP_INT LogPoly5(FP_INT a)
	{
		FP_INT y = Qmul30(a, 34835446); // 0.0324430374324099257645920506145091908173169505782530351933872568452187970039716570286755191899094832608276898590172296967
		y = Qmul30(a, y + -149023176); // -0.138788648453891138663259214948877985710758551758834443319382469349215457727435900740974302256302169487791331019735819359
		y = Qmul30(a, y + 315630515); // 0.293953823490661881198275484636301174125635657531784266710660462843103551518793294127904893642006610982580864921150987405
		y = Qmul30(a, y + -530763208); // -0.494311758014893283441267083938639942320000878038159975670627731298854764041757060977673894877216298188619864197654458825
		y = Qmul30(a, y + 1073581542); // 0.999850726105657924558890885094884131163156956047212371206642490453141495216122729917931111298021060974997472559962156274
		return y;
	}

	static FP_INT LogPoly3Lut4Table[] =
	{
		270509931, -528507852, 1073614348, 0,
		139305305, -442070189, 1053671695, 1633382,
		83615845, -360802306, 1013781196, 8222843,
		52639154, -291267388, 961502851, 21386502,
	};

	// Precision: 12.51 bits
	static FP_INT LogPoly3Lut4(FP_INT a)
	{
		FP_INT offset = (a >> 28) * 4;
		FP_INT y = Qmul30(a, LogPoly3Lut4Table[offset + 0]);
		y = Qmul30(a, y + LogPoly3Lut4Table[offset + 1]);
		y = Qmul30(a, y + LogPoly3Lut4Table[offset + 2]);
		y = y + LogPoly3Lut4Table[offset + 3];
		return y;
	}

	static FP_INT LogPoly3Lut8Table[] =
	{
		309628536, -534507419, 1073724054, 0,
		215207992, -502390266, 1069897914, 160852,
		158892020, -461029083, 1059680319, 1010114,
		120758300, -418592578, 1043877151, 2979626,
		93932535, -378620013, 1023979692, 6288435,
		74487828, -342313729, 1001351633, 10996073,
		60012334, -309817259, 977010327, 17079637,
		48377690, -279159893, 950059138, 24984183,
	};

	// Precision: 15.35 bits
	static FP_INT LogPoly3Lut8(FP_INT a)
	{
		FP_INT offset = (a >> 27) * 4;
		FP_INT y = Qmul30(a, LogPoly3Lut8Table[offset + 0]);
		y = Qmul30(a, y + LogPoly3Lut8Table[offset + 1]);
		y = Qmul30(a, y + LogPoly3Lut8Table[offset + 2]);
		y = y + LogPoly3Lut8Table[offset + 3];
		return y;
	}

	static FP_INT LogPoly5Lut8Table[] =
	{
		166189159, -263271008, 357682461, -536867223, 1073741814, 0,
		91797130, -221452381, 347549389, -535551692, 1073651718, 2559,
		55429773, -177286543, 325776420, -530104991, 1072960646, 38103,
		35101911, -139778071, 297915163, -519690478, 1071001695, 186416,
		23102252, -110088504, 268427087, -504993810, 1067326167, 555414,
		15701243, -87124604, 239861114, -487185708, 1061762610, 1252264,
		10960108, -69430156, 213404033, -467374507, 1054333366, 2368437,
		7703441, -55178389, 188423866, -445453304, 1044702281, 4063226,
	};

	// Precision: 26.22 bits
	static FP_INT LogPoly5Lut8(FP_INT a)
	{
		FP_INT offset = (a >> 27) * 6;
		FP_INT y = Qmul30(a, LogPoly5Lut8Table[offset + 0]);
		y = Qmul30(a, y + LogPoly5Lut8Table[offset + 1]);
		y = Qmul30(a, y + LogPoly5Lut8Table[offset + 2]);
		y = Qmul30(a, y + LogPoly5Lut8Table[offset + 3]);
		y = Qmul30(a, y + LogPoly5Lut8Table[offset + 4]);
		y = y + LogPoly5Lut8Table[offset + 5];
		return y;
	}

    // Log2()

	// Precision: 12.29 bits
	static FP_INT Log2Poly5(FP_INT a)
	{
		FP_INT y = Qmul30(a, 47840369); // 0.0445548155276207896995334754162140597637031202974591126199168774393873986289641382244343408731171726931757539068975485089
		y = Qmul30(a, y + -208941842); // -0.194592255208938416591621284205816720732140050852301947258138293025978577320103558315407526014074332839410207729682281855
		y = Qmul30(a, y + 450346773); // 0.419418116511448143225544710148490988337404380945888758986024844824480954559055561814904948371254539592688384332290775469
		y = Qmul30(a, y + -764275149); // -0.711786700405071059895411856470396704111669190613765834655920030051222814749610509844938951593547905280264475803542602324
		y = Qmul30(a, y + 1548771675); // 1.44240602357494054356195495511150837674248533596658656579701261211118275506108037663799064380674298602249584492474438398
		return y;
	}

	static FP_INT Log2Poly4Lut4Table[] =
	{
		-262388804, 497357316, -773551400, 1549073482, 0,
		-109627834, 364448809, -727169110, 1541348674, 512282,
		-55606812, 259947350, -650393145, 1515947800, 3705096,
		-30193295, 184276844, -565362946, 1473209058, 11812165,
	};

	// Precision: 17.47 bits
	static FP_INT Log2Poly4Lut4(FP_INT a)
	{
		FP_INT offset = (a >> 28) * 5;
		FP_INT y = Qmul30(a, Log2Poly4Lut4Table[offset + 0]);
		y = Qmul30(a, y + Log2Poly4Lut4Table[offset + 1]);
		y = Qmul30(a, y + Log2Poly4Lut4Table[offset + 2]);
		y = Qmul30(a, y + Log2Poly4Lut4Table[offset + 3]);
		y = y + Log2Poly4Lut4Table[offset + 4];
		return y;
	}

	static FP_INT Log2Poly5Lut4Table[] =
	{
		188232988, -362436158, 514145569, -774469188, 1549081618, 0,
		63930491, -229184904, 452495120, -759064000, 1547029186, 114449,
		27404630, -141534019, 367122541, -716855295, 1536437358, 1193011,
		12852334, -87700426, 286896922, -656644341, 1513678972, 4658365,
	};

	// Precision: 21.93 bits
	static FP_INT Log2Poly5Lut4(FP_INT a)
	{
		FP_INT offset = (a >> 28) * 6;
		FP_INT y = Qmul30(a, Log2Poly5Lut4Table[offset + 0]);
		y = Qmul30(a, y + Log2Poly5Lut4Table[offset + 1]);
		y = Qmul30(a, y + Log2Poly5Lut4Table[offset + 2]);
		y = Qmul30(a, y + Log2Poly5Lut4Table[offset + 3]);
		y = Qmul30(a, y + Log2Poly5Lut4Table[offset + 4]);
		y = y + Log2Poly5Lut4Table[offset + 5];
		return y;
	}

	static FP_INT Log2Poly3Lut8Table[] =
	{
		446326382, -771076074, 1549055308, 0,
		310260104, -724673704, 1543514571, 233309,
		229088935, -664989874, 1528754169, 1461470,
		174118266, -603771378, 1505939900, 4306814,
		135444733, -546112897, 1477222993, 9084839,
		107410065, -493744566, 1444569702, 15881168,
		86538496, -446871661, 1409446548, 24662718,
		69761446, -402649011, 1370556774, 36072616,
	};

	// Precision: 15.82 bits
	static FP_INT Log2Poly3Lut8(FP_INT a)
	{
		FP_INT offset = (a >> 27) * 4;
		FP_INT y = Qmul30(a, Log2Poly3Lut8Table[offset + 0]);
		y = Qmul30(a, y + Log2Poly3Lut8Table[offset + 1]);
		y = Qmul30(a, y + Log2Poly3Lut8Table[offset + 2]);
		y = y + Log2Poly3Lut8Table[offset + 3];
		return y;
	}

	static FP_INT Log2Poly3Lut16Table[] =
	{
		479498023, -773622327, 1549078527, 0,
		395931761, -759118188, 1548197526, 18808,
		334661898, -736470659, 1545381846, 136568,
		285596493, -709076642, 1540263722, 456574,
		245720905, -679311878, 1532841693, 1074840,
		212953734, -648695298, 1523292726, 2068966,
		185770248, -618189987, 1511870714, 3495916,
		163026328, -588395848, 1498851584, 5393582,
		143849516, -559673988, 1484504546, 7783737,
		127565758, -532227925, 1469077963, 10675243,
		113648249, -506157040, 1452793288, 14067055,
		101680803, -481491750, 1435843119, 17950929,
		91330868, -458215848, 1418390572, 22314023,
		82328154, -436276909, 1400565714, 27142441,
		74439828, -415566448, 1382437636, 32432624,
		67062062, -394757211, 1362869483, 38567491,
	};

	// Precision: 18.77 bits
	static FP_INT Log2Poly3Lut16(FP_INT a)
	{
		FP_INT offset = (a >> 26) * 4;
		FP_INT y = Qmul30(a, Log2Poly3Lut16Table[offset + 0]);
		y = Qmul30(a, y + Log2Poly3Lut16Table[offset + 1]);
		y = Qmul30(a, y + Log2Poly3Lut16Table[offset + 2]);
		y = y + Log2Poly3Lut16Table[offset + 3];
		return y;
	}

	static FP_INT Log2Poly4Lut16Table[] =
	{
		-349683705, 514860252, -774521507, 1549081965, 0,
		-271658431, 496776802, -772844764, 1549008620, 1259,
		-217158937, 469966332, -767835780, 1548587446, 14699,
		-175799370, 439219304, -759216789, 1547507699, 65699,
		-143866844, 407471403, -747343665, 1545528123, 189847,
		-118877791, 376365258, -732794890, 1542497870, 426993,
		-99090809, 346778829, -716182669, 1538346679, 816522,
		-83256460, 319137771, -698070351, 1533066538, 1394329,
		-70462839, 293601763, -678942086, 1526693477, 2191193,
		-60034672, 270176585, -659197359, 1519292323, 3232171,
		-51465396, 248781811, -639156567, 1510944906, 4536639,
		-44370441, 229291517, -619070546, 1501741200, 6118756,
		-38454405, 211558058, -599130091, 1491772420, 7988267,
		-33487114, 195423423, -579471329, 1481123710, 10151959,
		-29282549, 180709967, -560158338, 1469854024, 12618653,
		-25515190, 166551747, -540200057, 1457346639, 15558687,
	};

	// Precision: 25.20 bits
	static FP_INT Log2Poly4Lut16(FP_INT a)
	{
		FP_INT offset = (a >> 26) * 5;
		FP_INT y = Qmul30(a, Log2Poly4Lut16Table[offset + 0]);
		y = Qmul30(a, y + Log2Poly4Lut16Table[offset + 1]);
		y = Qmul30(a, y + Log2Poly4Lut16Table[offset + 2]);
		y = Qmul30(a, y + Log2Poly4Lut16Table[offset + 3]);
		y = y + Log2Poly4Lut16Table[offset + 4];
		return y;
	}

    // Sin()

	// Precision: 12.55 bits
	static FP_INT SinPoly2(FP_INT a)
	{
		FP_INT y = Qmul30(a, 78160664); // 0.072792791246675240806633584756838912025391316324690126147664432597740012658387971002826696503964998382073099859493224924
		y = Qmul30(a, y + -691048553); // -0.643589118041571860037955276396590354123911419602492412009771153095258421228154501762591444328997849123819708031503216569
		y = y + 1686629713; // 1.57079632679489661923132169163975144209852010327780228586210672049751840856976653075976474782503285074174660817200999164
		return y;
	}

	// Precision: 19.56 bits
	static FP_INT SinPoly3(FP_INT a)
	{
		FP_INT y = Qmul30(a, -4685819); // -0.00436400981703153243210864997931625819052350492882668525242722064533389220603470732171385204753335364507030843902034709469
		y = Qmul30(a, y + 85358772); // 0.0794965509242783578799016950654626792792298788902324903830739535612665082075477776612291621671450318813032241372211405835
		y = Qmul30(a, y + -693560840); // -0.645928867902143444679114736725897863187226477239208090992753453413451024571279601099280057944644528977979523870210785134
		y = y + 1686629713; // 1.57079632679489661923132169163975144209852010327780228586210672049751840856976653075976474782503285074174660817200999164
		return y;
	}

	// Precision: 27.13 bits
	static FP_INT SinPoly4(FP_INT a)
	{
		FP_INT y = Qmul30(a, 162679); // 0.000151506641710145430212560273580165931825591912723771559939880958777921352896251494433561036087921925941339032487946104446
		y = Qmul30(a, y + -5018587); // -0.0046739239118693360423625115440933405485555388758012378155538229669555462190128366781129325889847935291248353457031014355
		y = Qmul30(a, y + 85566362); // 0.0796898846149471415166275702814129714699583291426024010731469442497475447581642697337742897122044339073717901878121832219
		y = Qmul30(a, y + -693598342); // -0.645963794139684570135799310650651238951748485457327220679639722739088328461814215309859665984340413045934902046607019536
		y = y + 1686629713; // 1.57079632679489661923132169163975144209852010327780228586210672049751840856976653075976474782503285074174660817200999164
		return y;
	}

    // Atan()

	// Precision: 11.51 bits
	static FP_INT AtanPoly4(FP_INT a)
	{
		FP_INT y = Qmul30(a, 160726798); // 0.149688495302819745936382180128149414212975169816783327757105073455364913850052796368792673611118203908491930788482514717
		y = Qmul30(a, y + -389730008); // -0.3629643552067315751294669187222720090413427534177140297655271624082990667114095804438257977266614399793827935382192301
		y = Qmul30(a, y + -1791887); // -0.00166882600835556487643157555251563341464517039273944961175492629580374127544152356827950414733023151629754542508176777682
		y = Qmul30(a, y + 1074109956); // 1.00034284930971570368517715996651394929230510383744660686391316332569199570835055730032133459840273458272542980313905982
		return y;
	}

	static FP_INT AtanPoly5Lut8Table[] =
	{
		204464916, 1544566, -357994250, 1395, 1073741820, 0,
		119369854, 56362968, -372884915, 2107694, 1073588633, 4534,
		10771151, 190921163, -440520632, 19339556, 1071365339, 120610,
		-64491917, 329189978, -542756389, 57373179, 1064246365, 656900,
		-89925028, 390367074, -601765924, 85907899, 1057328034, 1329793,
		-80805750, 360696628, -563142238, 60762238, 1065515580, 263159,
		-58345538, 276259197, -435975641, -35140679, 1101731779, -5215389,
		-36116738, 179244146, -266417331, -183483381, 1166696761, -16608596,
        0, 0, 0, 0, 0, 843314857 // Atan(1.0)
	};

	// Precision: 28.06 bits
	static FP_INT AtanPoly5Lut8(FP_INT a)
	{
		FP_INT offset = (a >> 27) * 6;
		FP_INT y = Qmul30(a, AtanPoly5Lut8Table[offset + 0]);
		y = Qmul30(a, y + AtanPoly5Lut8Table[offset + 1]);
		y = Qmul30(a, y + AtanPoly5Lut8Table[offset + 2]);
		y = Qmul30(a, y + AtanPoly5Lut8Table[offset + 3]);
		y = Qmul30(a, y + AtanPoly5Lut8Table[offset + 4]);
		y = y + AtanPoly5Lut8Table[offset + 5];
		return y;
	}

	static FP_INT AtanPoly3Lut8Table[] =
	{
		-351150132, -463916, 1073745980, 0,
		-289359685, -24349242, 1076929105, -145366,
		-192305259, -97257464, 1095342438, -1708411,
		-91138684, -210466171, 1137733496, -7020039,
		-8856969, -332956892, 1198647251, -17139451,
		46187514, -435267135, 1262120294, -30283758,
		76277334, -502284461, 1311919661, -42630181,
		88081006, -532824470, 1338273149, -50214826,
        0, 0, 0, 843314857 // Atan(1.0)
    };

	// Precision: 17.98 bits
	static FP_INT AtanPoly3Lut8(FP_INT a)
	{
		FP_INT offset = (a >> 27) * 4;
		FP_INT y = Qmul30(a, AtanPoly3Lut8Table[offset + 0]);
		y = Qmul30(a, y + AtanPoly3Lut8Table[offset + 1]);
		y = Qmul30(a, y + AtanPoly3Lut8Table[offset + 2]);
		y = y + AtanPoly3Lut8Table[offset + 3];
		return y;
	}
    static const FP_INT Shift = 32;
    static const FP_LONG FractionMask = ( INT64_C(1) << Shift ) - 1; // Space before INT64_C(1) needed because of hacky C++ code generator
    static const FP_LONG IntegerMask = ~FractionMask;

    // Constants
    static const FP_LONG Zero = INT64_C(0);
    static const FP_LONG Neg1 = INT64_C(-1) << Shift;
    static const FP_LONG One = INT64_C(1) << Shift;
    static const FP_LONG Two = INT64_C(2) << Shift;
    static const FP_LONG Three = INT64_C(3) << Shift;
    static const FP_LONG Four = INT64_C(4) << Shift;
    static const FP_LONG Half = One >> 1;
    static const FP_LONG Pi = INT64_C(13493037705); //(FP_LONG)(FP_PI * 65536.0) << 16;
    static const FP_LONG Pi2 = INT64_C(26986075409);
    static const FP_LONG PiHalf = INT64_C(6746518852);
    static const FP_LONG E = INT64_C(11674931555);

    static const FP_LONG MinValue = INT64_C(-9223372036854775808); // INT64_C(0x8000000000000000)
    static const FP_LONG MaxValue = INT64_C(0x7FFFFFFFFFFFFFFF);

    // Private constants
    static const FP_LONG RCP_LN2      = INT64_C(0x171547652); // 1.0 / log(2.0) ~= 1.4426950408889634
    static const FP_LONG RCP_LOG2_E   = INT64_C(2977044471);  // 1.0 / log2(e) ~= 0.6931471805599453

    /// <summary>
    /// Converts an integer to a fixed-point value.
    /// </summary>
    static FP_LONG FromInt(FP_INT v)
    {
        return (FP_LONG)v << Shift;
    }

    /// <summary>
    /// Converts a double to a fixed-point value.
    /// </summary>
    static FP_LONG FromDouble(double v)
    {
        return (FP_LONG)(v * 4294967296.0);
    }

    /// <summary>
    /// Converts a float to a fixed-point value.
    /// </summary>
    static FP_LONG FromFloat(float v)
    {
        return FromDouble((double)v);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it up to nearest integer.
    /// </summary>
    static FP_INT CeilToInt(FP_LONG v)
    {
        return (FP_INT)((v + (One - 1)) >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it down to nearest integer.
    /// </summary>
    static FP_INT FloorToInt(FP_LONG v)
    {
        return (FP_INT)(v >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into an integer by rounding it to nearest integer.
    /// </summary>
    static FP_INT RoundToInt(FP_LONG v)
    {
        return (FP_INT)((v + Half) >> Shift);
    }

    /// <summary>
    /// Converts a fixed-point value into a double.
    /// </summary>
    static double ToDouble(FP_LONG v)
    {
        return (double)v * (1.0 / 4294967296.0);
    }

    /// <summary>
    /// Converts a FP value into a float.
    /// </summary>
    static float ToFloat(FP_LONG v)
    {
        return (float)v * (1.0f / 4294967296.0f);
    }

    /// <summary>
    /// Converts the value to a human readable string.
    /// </summary>

    /// <summary>
    /// Returns the absolute (positive) value of x.
    /// </summary>
    static FP_LONG Abs(FP_LONG x)
    {
        // \note fails with LONG_MIN
        // \note for some reason this is twice as fast as (x > 0) ? x : -x
        return (x < 0) ? -x : x;
    }

    /// <summary>
    /// Negative absolute value (returns -abs(x)).
    /// </summary>
    static FP_LONG Nabs(FP_LONG x)
    {
        return (x > 0) ? -x : x;
    }

    /// <summary>
    /// Round up to nearest integer.
    /// </summary>
    static FP_LONG Ceil(FP_LONG x)
    {
        return (x + FractionMask) & IntegerMask;
    }

    /// <summary>
    /// Round down to nearest integer.
    /// </summary>
    static FP_LONG Floor(FP_LONG x)
    {
        return x & IntegerMask;
    }

    /// <summary>
    /// Round to nearest integer.
    /// </summary>
    static FP_LONG Round(FP_LONG x)
    {
        return (x + Half) & IntegerMask;
    }

    /// <summary>
    /// Returns the fractional part of x. Equal to 'x - floor(x)'.
    /// </summary>
    static FP_LONG Fract(FP_LONG x)
    {
        return x & FractionMask;
    }

    /// <summary>
    /// Returns the minimum of the two values.
    /// </summary>
    static FP_LONG Min(FP_LONG a, FP_LONG b)
    {
        return (a < b) ? a : b;
    }

    /// <summary>
    /// Returns the maximum of the two values.
    /// </summary>
    static FP_LONG Max(FP_LONG a, FP_LONG b)
    {
        return (a > b) ? a : b;
    }

    /// <summary>
    /// Returns the sign of the value (-1 if negative, 0 if zero, 1 if positive).
    /// </summary>
    static FP_INT Sign(FP_LONG x)
    {
        if (x == 0) return 0;
        return (x < 0) ? -1 : 1;
    }

    /// <summary>
    /// Adds the two FP numbers together.
    /// </summary>
    static FP_LONG Add(FP_LONG a, FP_LONG b)
    {
        return a + b;
    }

    /// <summary>
    /// Subtracts the two FP numbers from each other.
    /// </summary>
    static FP_LONG Sub(FP_LONG a, FP_LONG b)
    {
        return a - b;
    }

    /// <summary>
    /// Multiplies two FP values together.
    /// </summary>
    static FP_LONG Mul(FP_LONG a, FP_LONG b)
    {
        /*FP_ULONG alow = (FP_ULONG)(a & FP_Mask_Fract);
        FP_LONG ahigh = a >> FP_Shift;
        FP_ULONG blow = (FP_ULONG)(b & FP_Mask_Fract);
        FP_LONG bhigh = b >> FP_Shift;

        FP_ULONG lowlow = alow * blow;
        FP_LONG lowhigh = (FP_LONG)(uint)alow * bhigh;
        FP_LONG highlow = ahigh * (FP_LONG)(uint)blow;
        FP_LONG highhigh = ahigh * bhigh;

        FP_LONG lo_res = (FP_LONG)(lowlow >> 32);
        FP_LONG mid_res1 = lowhigh;
        FP_LONG mid_res2 = highlow;
        FP_LONG hi_res = highhigh << 32;

        return lo_res + mid_res1 + mid_res2 + hi_res;*/

        FP_LONG ai = a >> Shift;
        FP_ULONG af = (FP_ULONG)(a & FractionMask);
        FP_LONG bi = b >> Shift;
        FP_ULONG bf = (FP_ULONG)(b & FractionMask);

        return
            (FP_LONG)((af * bf) >> Shift) +
            ai * b +
            (FP_LONG)af * bi;
    }

    static FP_INT MulIntLongLow(FP_INT a, FP_LONG b)
    {
        FP_ASSERT(a >= 0);
        FP_ULONG af = (FP_ULONG)a;
        FP_LONG bi = b >> Shift;
        FP_ULONG bf = (FP_ULONG)(b & FractionMask);

        return (FP_INT)((FP_LONG)((af * bf) >> Shift) + (FP_LONG)af * bi);
    }

    static FP_INT Nlz(FP_ULONG x)
    {
        FP_INT n = 0;
        if (x <= INT64_C(0x00000000FFFFFFFF)) { n = n + 32; x = x << 32; }
        if (x <= INT64_C(0x0000FFFFFFFFFFFF)) { n = n + 16; x = x << 16; }
        if (x <= INT64_C(0x00FFFFFFFFFFFFFF)) { n = n + 8; x = x << 8; }
        if (x <= INT64_C(0x0FFFFFFFFFFFFFFF)) { n = n + 4; x = x << 4; }
        if (x <= INT64_C(0x3FFFFFFFFFFFFFFF)) { n = n + 2; x = x << 2; }
        if (x <= INT64_C(0x7FFFFFFFFFFFFFFF)) { n = n + 1; }
        if (x == 0) return 64;
        return n;
    }

    static FP_LONG DivRem(FP_LONG arg_a, FP_LONG arg_b, FP_LONG &rem)
    {
        // From http://www.hackersdelight.org/hdcodetxt/divlu.c.txt

        FP_LONG sign_dif = arg_a ^ arg_b;

        static const FP_ULONG b = INT64_C(0x100000000); // Number base (32 bits)
        FP_ULONG unsigned_arg_a = (FP_ULONG)((arg_a < 0) ? -arg_a : arg_a);
        FP_ULONG u1 = unsigned_arg_a >> 32;
        FP_ULONG u0 = unsigned_arg_a << 32;
        FP_ULONG v = (FP_ULONG)((arg_b < 0) ? -arg_b : arg_b);

        // Overflow?
        if (u1 >= v)
        {
            rem = 0;
            return INT64_C(0x7fffffffffffffff);
        }

        // Shift amount for norm
        FP_INT s = Nlz(v); // 0 <= s <= 63
        v = v << s; // Normalize the divisor
        FP_ULONG vn1 = v >> 32; // Break the divisor into two 32-bit digits
        FP_ULONG vn0 = v & INT64_C(0xffffffff);

        FP_ULONG un32 = (u1 << s) | (u0 >> (64 - s)) & (FP_ULONG)((FP_LONG)-s >> 63);
        FP_ULONG un10 = u0 << s; // Shift dividend left

        FP_ULONG un1 = un10 >> 32; // Break the right half of dividend into two digits
        FP_ULONG un0 = un10 & INT64_C(0xffffffff);

        // Compute the first quotient digit, q1
        FP_ULONG q1 = un32 / vn1;
        FP_ULONG rhat = un32 - q1 * vn1;
        do
        {
            if ((q1 >= b) || ((q1 * vn0) > (b * rhat + un1)))
            {
                q1 = q1 - 1;
                rhat = rhat + vn1;
            } else break;
        } while (rhat < b);

        FP_ULONG un21 = un32 * b + un1 - q1 * v; // Multiply and subtract

        // Compute the second quotient digit, q0
        FP_ULONG q0 = un21 / vn1;
        rhat = un21 - q0 * vn1;
        do
        {
            if ((q0 >= b) || ((q0 * vn0) > (b * rhat + un0)))
            {
                q0 = q0 - 1;
                rhat = rhat + vn1;
            } else break;
        } while (rhat < b);

        // Calculate the remainder
        FP_ULONG r = (un21 * b + un0 - q0 * v) >> s;
        rem = (FP_LONG)r;

        FP_ULONG ret = q1 * b + q0;
        return (sign_dif < 0) ? -(FP_LONG)ret : (FP_LONG)ret;
    }

    /// <summary>
    /// Divides two FP values.
    /// </summary>
    static FP_LONG Div(FP_LONG arg_a, FP_LONG arg_b)
    {
        FP_LONG rem;
        return DivRem(arg_a, arg_b, rem);
    }

    /// <summary>
    /// Divides two FP values and returns the modulus.
    /// </summary>
    static FP_LONG Mod(FP_LONG a, FP_LONG b)
    {
        /*FP_LONG d = Div(a, b);
        FP_INT di = ToInt(d);
        FP_LONG ret = a - (di * b);
         * 
        // Sign difference?
        if ((a ^ b) < 0)
            return ret - b;
        return ret;*/

        //FP_LONG di = Div(a, b) >> FP_Shift;
        FP_LONG di = a / b;
        FP_LONG ret = a - (di * b);
        return ret;
    }

    /// <summary>
    /// Calculates the square root of the given number.
    /// </summary>
    static FP_LONG SqrtPrecise(FP_LONG a)
    {
        // Adapted from https://github.com/chmike/fpsqrt
        if (a < 0)
            return -1;

        FP_ULONG r = (FP_ULONG)a;
        FP_ULONG b = INT64_C(0x4000000000000000);
        FP_ULONG q = INT64_C(0);
        while (b > INT64_C(0x40))
        {
            FP_ULONG t = q + b;
            if (r >= t)
            {
                r -= t;
                q = t + b;
            }
            r <<= 1;
            b >>= 1;
        }
        q >>= 16;
        return (FP_LONG)q;
    }

    static FP_LONG Sqrt(FP_LONG x)
    {
        // Return 0 for all non-positive values.
        if (x <= 0)
            return 0;

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_INT y = SqrtPoly3Lut8(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul30(adjust, y) << 2;
        return (offset >= 0) ? (yr << offset) : (yr >> -offset);
    }

    static FP_LONG SqrtFast(FP_LONG x)
    {
        // Return 0 for all non-positive values.
        if (x <= 0)
            return 0;

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_INT y = SqrtPoly4(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul30(adjust, y) << 2;
        return (offset >= 0) ? (yr << offset) : (yr >> -offset);
    }

    static FP_LONG SqrtFastest(FP_LONG x)
    {
        // Return 0 for all non-positive values.
        if (x <= 0)
            return 0;

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT SQRT2 = 1518500249; // sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_INT y = SqrtPoly3(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul30(adjust, y) << 2;
        return (offset >= 0) ? (yr << offset) : (yr >> -offset);
    }

    /// <summary>
    /// Calculates the reciprocal square root.
    /// </summary>
    static FP_LONG RSqrt(FP_LONG x)
    {
        FP_ASSERT(x > 0);

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_INT y = RSqrtPoly3Lut16(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul30(adjust, y) << 2;
        return (offset >= 0) ? (yr >> offset) : (yr << -offset);
    }

    /// <summary>
    /// Calculates the reciprocal square root.
    /// </summary>
    static FP_LONG RSqrtFast(FP_LONG x)
    {
        FP_ASSERT(x > 0);

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_INT y = RSqrtPoly5(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul30(adjust, y) << 2;
        return (offset >= 0) ? (yr >> offset) : (yr << -offset);
    }

    /// <summary>
    /// Calculates the reciprocal square root.
    /// </summary>
    static FP_LONG RSqrtFastest(FP_LONG x)
    {
        FP_ASSERT(x > 0);

        // Constants (s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF_SQRT2 = 759250125; // 0.5 * sqrt(2.0)

        // Normalize input into [1.0, 2.0( range (as s2.30).
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_INT y = RSqrtPoly3(n - ONE);

        // Divide offset by 2 (to get sqrt), compute adjust value for odd exponents.
        FP_INT adjust = ((offset & 1) != 0) ? HALF_SQRT2 : ONE;
        offset = offset >> 1;

        // Apply exponent, convert back to s32.32.
        FP_LONG yr = (FP_LONG)Qmul30(adjust, y) << 2;
        return (offset >= 0) ? (yr >> offset) : (yr << -offset);
    }

    /// <summary>
    /// Calculates the reciprocal using precise division.
    /// </summary>
    static FP_LONG RcpDiv(FP_LONG a)
    {
        return Div(One, a);
    }

    /// <summary>
    /// Calculates reciprocal approximation.
    /// </summary>
    static FP_LONG Rcp(FP_LONG x)
    {
        if (x == MinValue)
            return 0;

        // Handle negative values.
        FP_INT sign = (x < 0) ? -1 : 1;
        x *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_INT k = n - ONE;

        // Polynomial approximation.
        FP_INT res = RcpPoly4Lut8(k);
        FP_LONG y = (FP_LONG)(sign * res) << 2;

        // Apply exponent, convert back to s32.32.
        return (offset >= 0) ? (y >> offset) : (y << -offset);
    }

    /// <summary>
    /// Calculates reciprocal approximation.
    /// </summary>
    static FP_LONG RcpFast(FP_LONG x)
    {
        if (x == MinValue)
            return 0;

        // Handle negative values.
        FP_INT sign = (x < 0) ? -1 : 1;
        x *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_INT k = n - ONE;

        // Polynomial approximation.
        FP_INT res = RcpPoly6(k);
        FP_LONG y = (FP_LONG)(sign * res) << 2;

        // Apply exponent, convert back to s32.32.
        return (offset >= 0) ? (y >> offset) : (y << -offset);
    }

    /// <summary>
    /// Calculates reciprocal approximation.
    /// </summary>
    static FP_LONG RcpFastest(FP_LONG x)
    {
        if (x == MinValue)
            return 0;

        // Handle negative values.
        FP_INT sign = (x < 0) ? -1 : 1;
        x *= sign;

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_INT k = n - ONE;

        // Polynomial approximation.
        FP_INT res = RcpPoly4(k);
        FP_LONG y = (FP_LONG)(sign * res) << 2;

        // Apply exponent, convert back to s32.32.
        return (offset >= 0) ? (y >> offset) : (y << -offset);
    }

    /// <summary>
    /// Calculates the base 2 exponent.
    /// </summary>
    static FP_LONG Exp2(FP_LONG x)
    {
        // Handle values that would under or overflow.
        if (x >= 32 * One) return MaxValue;
        if (x <= -32 * One) return 0;

        // Compute exp2 for fractional part.
        FP_INT k = (FP_INT)((x & FractionMask) >> 2);
        FP_LONG y = (FP_LONG)Exp2Poly5(k) << 2;

        // Combine integer and fractional result, and convert back to s32.32.
        FP_INT intPart = (FP_INT)(x >> 32);
        return (intPart >= 0) ? (y << intPart) : (y >> -intPart);
    }

    /// <summary>
    /// Calculates the base 2 exponent.
    /// </summary>
    static FP_LONG Exp2Fast(FP_LONG x)
    {
        // Handle values that would under or overflow.
        if (x >= 32 * One) return MaxValue;
        if (x <= -32 * One) return 0;

        // Compute exp2 for fractional part.
        FP_INT k = (FP_INT)((x & FractionMask) >> 2);
        FP_LONG y = (FP_LONG)Exp2Poly4(k) << 2;

        // Combine integer and fractional result, and convert back to s32.32.
        FP_INT intPart = (FP_INT)(x >> 32);
        return (intPart >= 0) ? (y << intPart) : (y >> -intPart);
    }

    /// <summary>
    /// Calculates the base 2 exponent.
    /// </summary>
    static FP_LONG Exp2Fastest(FP_LONG x)
    {
        // Handle values that would under or overflow.
        if (x >= 32 * One) return MaxValue;
        if (x <= -32 * One) return 0;

        // Compute exp2 for fractional part.
        FP_INT k = (FP_INT)((x & FractionMask) >> 2);
        FP_LONG y = (FP_LONG)Exp2Poly3(k) << 2;

        // Combine integer and fractional result, and convert back to s32.32.
        FP_INT intPart = (FP_INT)(x >> 32);
        return (intPart >= 0) ? (y << intPart) : (y >> -intPart);
    }

    static FP_LONG Exp(FP_LONG x)
    {
        // e^x == 2^(x / ln(2))
        return Exp2(Mul(x, RCP_LN2));
    }

    static FP_LONG ExpFast(FP_LONG x)
    {
        // e^x == 2^(x / ln(2))
        return Exp2Fast(Mul(x, RCP_LN2));
    }

    static FP_LONG ExpFastest(FP_LONG x)
    {
        // e^x == 2^(x / ln(2))
        return Exp2Fastest(Mul(x, RCP_LN2));
    }

    static FP_LONG Log(FP_LONG x)
    {
        // Natural logarithm (base e).
        FP_ASSERT(x > 0);

        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        static const FP_INT ONE = (1 << 30);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_LONG y = (FP_LONG)LogPoly5Lut8(n - ONE) << 2;

        // Combine integer and fractional parts (into s32.32).
        return (FP_LONG)offset * RCP_LOG2_E + y;
    }

    static FP_LONG LogFast(FP_LONG x)
    {
        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_ASSERT(x > 0);
        static const FP_INT ONE = (1 << 30);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_LONG y = (FP_LONG)LogPoly3Lut8(n - ONE) << 2;

        // Combine integer and fractional parts (into s32.32).
        return (FP_LONG)offset * RCP_LOG2_E + y;
    }

    static FP_LONG LogFastest(FP_LONG x)
    {
        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_ASSERT(x > 0);
        static const FP_INT ONE = (1 << 30);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_ASSERT(n >= ONE);
        FP_LONG y = (FP_LONG)LogPoly5(n - ONE) << 2;

        // Combine integer and fractional parts (into s32.32).
        return (FP_LONG)offset * RCP_LOG2_E + y;
    }

    static FP_LONG Log2(FP_LONG x)
    {
        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_ASSERT(x > 0);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

        // Polynomial approximation of mantissa.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_LONG y = (FP_LONG)Log2Poly4Lut16(n - ONE) << 2;

        // Combine integer and fractional parts (into s32.32).
        return ((FP_LONG)offset << Shift) + y;
    }

    static FP_LONG Log2Fast(FP_LONG x)
    {
        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_ASSERT(x > 0);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

        // Polynomial approximation of mantissa.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_LONG y = (FP_LONG)Log2Poly3Lut16(n - ONE) << 2;

        // Combine integer and fractional parts (into s32.32).
        return ((FP_LONG)offset << Shift) + y;
    }

    static FP_LONG Log2Fastest(FP_LONG x)
    {
        // Normalize value to range [1.0, 2.0( as s2.30 and extract exponent.
        FP_ASSERT(x > 0);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);

        // Polynomial approximation of mantissa.
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT(n >= ONE);
        FP_LONG y = (FP_LONG)Log2Poly5(n - ONE) << 2;

        // Combine integer and fractional parts (into s32.32).
        return ((FP_LONG)offset << Shift) + y;
    }

    /// <summary>
    /// Calculates x to the power of the exponent.
    /// </summary>
    static FP_LONG Pow(FP_LONG x, FP_LONG exponent)
    {
        FP_ASSERT(x >= 0);
        if (x <= 0) return 0;
        return Exp(Mul(exponent, Log(x)));
    }

    /// <summary>
    /// Calculates x to the power of the exponent.
    /// </summary>
    static FP_LONG PowFast(FP_LONG x, FP_LONG exponent)
    {
        FP_ASSERT(x >= 0);
        if (x <= 0) return 0;
        return ExpFast(Mul(exponent, LogFast(x)));
    }

    /// <summary>
    /// Calculates x to the power of the exponent.
    /// </summary>
    static FP_LONG PowFastest(FP_LONG x, FP_LONG exponent)
    {
        FP_ASSERT(x >= 0);
        if (x <= 0) return 0;
        return ExpFastest(Mul(exponent, LogFastest(x)));
    }

    static FP_LONG Sin(FP_LONG x)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Map [0, 2pi] to [0, 4] (as s2.30).
        // This also wraps the values into one period.
        static const FP_INT RCP_HALF_PI = 683565276; // 1.0 / (4.0 * 0.5 * FP_PI);  // the 4.0 factor converts directly to s2.30
        FP_INT z = MulIntLongLow(RCP_HALF_PI, x);

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        FP_INT zz = Qmul30(z, z);
        FP_INT res = Qmul30(SinPoly4(zz), z);

        // Convert back to s32.32.
        return (FP_LONG)res << 2;
    }

    static FP_LONG SinFast(FP_LONG x)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Map [0, 2pi] to [0, 4] (as s2.30).
        // This also wraps the values into one period.
        static const FP_INT RCP_HALF_PI = 683565276; // 1.0 / (4.0 * 0.5 * FP_PI);  // the 4.0 factor converts directly to s2.30
        FP_INT z = MulIntLongLow(RCP_HALF_PI, x);

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        FP_INT zz = Qmul30(z, z);
        FP_INT res = Qmul30(SinPoly3(zz), z);

        // Convert back to s32.32.
        return (FP_LONG)res << 2;
    }

    static FP_LONG SinFastest(FP_LONG x)
    {
        // See: http://www.coranac.com/2009/07/sines/

        // Map [0, 2pi] to [0, 4] (as s2.30).
        // This also wraps the values into one period.
        static const FP_INT RCP_HALF_PI = 683565276; // 1.0 / (4.0 * 0.5 * FP_PI);  // the 4.0 factor converts directly to s2.30
        FP_INT z = MulIntLongLow(RCP_HALF_PI, x);

        // Handle quadrants 1 and 2 by mirroring the [1, 3] range to [-1, 1] (by calculating 2 - z).
        // The if condition uses the fact that for the quadrants of interest are 0b01 and 0b10 (top two bits are different).
        if ((z ^ (z << 1)) < 0)
            z = (1 << 31) - z;

        // Now z is in range [-1, 1].
        static const FP_INT ONE = (1 << 30);
        FP_ASSERT((z >= -ONE) && (z <= ONE));

        // Polynomial approximation.
        FP_INT zz = Qmul30(z, z);
        FP_INT res = Qmul30(SinPoly2(zz), z);

        // Convert back to s32.32.
        return (FP_LONG)res << 2;
    }

    static FP_LONG Cos(FP_LONG x)
    {
        return Sin(x + PiHalf);
    }

    static FP_LONG CosFast(FP_LONG x)
    {
        return SinFast(x + PiHalf);
    }

    static FP_LONG CosFastest(FP_LONG x)
    {
        return SinFastest(x + PiHalf);
    }

    static FP_LONG Tan(FP_LONG x)
    {
        return Mul(Sin(x), Rcp(Cos(x)));
    }

    static FP_LONG TanFast(FP_LONG x)
    {
        return Mul(SinFast(x), RcpFast(CosFast(x)));
    }

    static FP_LONG TanFastest(FP_LONG x)
    {
        return Mul(SinFastest(x), RcpFastest(CosFastest(x)));
    }

    static FP_INT Atan2Div(FP_LONG y, FP_LONG x)
    {
        FP_ASSERT(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF = (1 << 29);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_INT k = n - ONE;

        // Polynomial approximation of reciprocal.
        FP_INT oox = RcpPoly4Lut8(k);
        FP_ASSERT(oox >= HALF && oox <= ONE);

        // Apply exponent and multiply.
        FP_LONG yr = (offset >= 0) ? (y >> offset) : (y << -offset);
        return Qmul30((FP_INT)(yr >> 2), oox);
    }

    static FP_LONG Atan2(FP_LONG y, FP_LONG x)
    {
        // See: https://www.dsprelated.com/showarticle/1052.php

        if (x == 0)
        {
            if (y > 0) return PiHalf;
            if (y < 0) return -PiHalf;
            return 0;
        }

        // \note these round negative numbers slightly
        FP_LONG nx = x ^ (x >> 63);
        FP_LONG ny = y ^ (y >> 63);
        FP_LONG negMask = ((x ^ y) >> 63);

        if (nx >= ny)
        {
            FP_INT k = Atan2Div(ny, nx);
            FP_INT z = AtanPoly5Lut8(k);
            FP_LONG angle = negMask ^ ((FP_LONG)z << 2);
            if (x > 0) return angle;
            if (y > 0) return angle + Pi;
            return angle - Pi;
        }
        else
        {
            FP_INT k = Atan2Div(nx, ny);
            FP_INT z = AtanPoly5Lut8(k);
            FP_LONG angle = negMask ^ ((FP_LONG)z << 2);
            return ((y > 0) ? PiHalf : -PiHalf) - angle;
        }
    }

    static FP_INT Atan2DivFast(FP_LONG y, FP_LONG x)
    {
        FP_ASSERT(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF = (1 << 29);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_INT k = n - ONE;

        // Polynomial approximation.
        FP_INT oox = RcpPoly6(k);
        FP_ASSERT(oox >= HALF && oox <= ONE);

        // Apply exponent and multiply.
        FP_LONG yr = (offset >= 0) ? (y >> offset) : (y << -offset);
        return Qmul30((FP_INT)(yr >> 2), oox);
    }

    static FP_LONG Atan2Fast(FP_LONG y, FP_LONG x)
    {
        // See: https://www.dsprelated.com/showarticle/1052.php

        if (x == 0)
        {
            if (y > 0) return PiHalf;
            if (y < 0) return -PiHalf;
            return 0;
        }

        // \note these round negative numbers slightly
        FP_LONG nx = x ^ (x >> 63);
        FP_LONG ny = y ^ (y >> 63);
        FP_LONG negMask = ((x ^ y) >> 63);

        if (nx >= ny)
        {
            FP_INT k = Atan2DivFast(ny, nx);
            FP_INT z = AtanPoly3Lut8(k);
            FP_LONG angle = negMask ^ ((FP_LONG)z << 2);
            if (x > 0) return angle;
            if (y > 0) return angle + Pi;
            return angle - Pi;
        }
        else
        {
            FP_INT k = Atan2DivFast(nx, ny);
            FP_INT z = AtanPoly3Lut8(k);
            FP_LONG angle = negMask ^ ((FP_LONG)z << 2);
            return ((y > 0) ? PiHalf : -PiHalf) - angle;
        }
    }

    static FP_INT Atan2DivFastest(FP_LONG y, FP_LONG x)
    {
        FP_ASSERT(y >= 0 && x > 0 && x >= y);

        // Normalize input into [1.0, 2.0( range (convert to s2.30).
        static const FP_INT ONE = (1 << 30);
        static const FP_INT HALF = (1 << 29);
        FP_INT offset = 31 - Nlz((FP_ULONG)x);
        FP_INT n = (FP_INT)(((offset >= 0) ? (x >> offset) : (x << -offset)) >> 2);
        FP_INT k = n - ONE;

        // Polynomial approximation.
        FP_INT oox = RcpPoly4(k);
        FP_ASSERT(oox >= HALF && oox <= ONE);

        // Apply exponent and multiply.
        FP_LONG yr = (offset >= 0) ? (y >> offset) : (y << -offset);
        return Qmul30((FP_INT)(yr >> 2), oox);
    }

    static FP_LONG Atan2Fastest(FP_LONG y, FP_LONG x)
    {
        // See: https://www.dsprelated.com/showarticle/1052.php

        if (x == 0)
        {
            if (y > 0) return PiHalf;
            if (y < 0) return -PiHalf;
            return 0;
        }

        // \note these round negative numbers slightly
        FP_LONG nx = x ^ (x >> 63);
        FP_LONG ny = y ^ (y >> 63);
        FP_LONG negMask = ((x ^ y) >> 63);

        if (nx >= ny)
        {
            FP_INT z = Atan2DivFastest(ny, nx);
            FP_INT res = AtanPoly4(z);
            FP_LONG angle = negMask ^ ((FP_LONG)res << 2);
            if (x > 0) return angle;
            if (y > 0) return angle + Pi;
            return angle - Pi;
        }
        else
        {
            FP_INT z = Atan2DivFastest(nx, ny);
            FP_INT res = AtanPoly4(z);
            FP_LONG angle = negMask ^ ((FP_LONG)res << 2);
            return ((y > 0) ? PiHalf : -PiHalf) - angle;
        }
    }

    static FP_LONG Asin(FP_LONG x)
    {
        FP_ASSERT(x >= -One && x <= One);
        return Atan2(x, Sqrt(Mul(One + x, One - x)));
    }

    static FP_LONG AsinFast(FP_LONG x)
    {
        FP_ASSERT(x >= -One && x <= One);
        return Atan2Fast(x, SqrtFast(Mul(One + x, One - x)));
    }

    static FP_LONG AsinFastest(FP_LONG x)
    {
        FP_ASSERT(x >= -One && x <= One);
        return Atan2Fastest(x, SqrtFastest(Mul(One + x, One - x)));
    }

    static FP_LONG Acos(FP_LONG x)
    {
        FP_ASSERT(x >= -One && x <= One);
        return Atan2(Sqrt(Mul(One + x, One - x)), x);
    }

    static FP_LONG AcosFast(FP_LONG x)
    {
        FP_ASSERT(x >= -One && x <= One);
        return Atan2Fast(SqrtFast(Mul(One + x, One - x)), x);
    }

    static FP_LONG AcosFastest(FP_LONG x)
    {
        FP_ASSERT(x >= -One && x <= One);
        return Atan2Fastest(SqrtFastest(Mul(One + x, One - x)), x);
    }

    static FP_LONG Atan(FP_LONG x)
    {
        return Atan2(x, One);
    }

    static FP_LONG AtanFast(FP_LONG x)
    {
        return Atan2Fast(x, One);
    }

    static FP_LONG AtanFastest(FP_LONG x)
    {
        return Atan2Fastest(x, One);
    }


    #undef FP_ASSERT
};
#endif

