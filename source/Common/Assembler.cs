using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    class Assembler
    {
        string test =
@"
//Variables:
@A = 1001
@B = 1002
@ACC = 1003
@COUNT = 1004
@LT = 1005
@REST = 1006

//Program:
//Implement A / B using addition
INPUT $A
INPUT $B
ADD 0 0 $ACC
ADD 0 0 $COUNT
:DivisionLoop (addr 12)
ADD $B $ACC $ACC
ADD 1 $COUNT $COUNT
LESSTHAN $ACC $A $LT$
JUMPIFTRUE $LT :DivisionLoop
//done adding, check if we overran target
EQUALS $ACC $A $REST
JUMPIFTRUE $REST :Done
ADD -1 $COUNT $COUNT
:Done
OUTPUT $COUNT
";
        string assembled =
@"
003,1001,
003,1002,
1101,0,0,1003,
1101,0,0,1004,
0001,1002,1003,1003
0101,1,1004,1004,
0007,1003,1001,1005,
1005,1005,12,
0008,1003,1001,1006
1005,1006,38
0101,-1,1004,1004,
0004,1004
";
    }
}
