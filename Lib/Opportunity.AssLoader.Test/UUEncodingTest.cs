using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;
using System;
using System.IO;
using System.Text;

namespace Opportunity.AssLoader.Test
{
    [TestClass]
    public class UUEncodingTest
    {
        public const string DATA = @"7V.D=GFQ>#""*<G:P81I\)&2I;8-A:GFM:3""J=S""H:7ZF=G&U:71A9HEA18.T4'^B:'6S#DMA;(2U=(-[
,S^H;82I>7)O9W^N,U^Q='^S>(6O;82Z4'FV,U&T=SV-<W&E:8)+#F.D=GFQ>&2Z='5[)(9U,D!Q+QJ5
;82M:4IA>7ZU;82M:71+4X*J:WFO97QA5W.S;8""U/C""V<GNO<X>O#E.P<'RJ=WFP<H-[)&*F>G6S=W5+
5'RB?6*F=VE[)$=S-!J1<'&Z5G6T7$IA-4)Y-!J8=G&Q5X2Z<'5[)$!+5W.B<'6E1G^S:'6S17ZE5WBB
:'^X/C""/<QJ4?7ZD;#""1<WFO>$IA-#YQ-1J5;7VF=DIA-4!Q,D!Q-$%+#F:J:'6P)%&T='6D>#""3982J
<TIA-!J7;72F<S"";<W^N/C!W#F:J:'6P)&""P=WFU;7^O/C!Q#ERB=X1A5X2Z<'5A5X2P=G&H:4IA68""P
>(2F#AJ<6D1L)&.U?7RF=VU+2G^S<7&U/C""/97VF,%:P<H2O97VF,%:P<H2T;8JF,&""S;7VB=HF$<WRP
>8)M5W6D<WZE98*Z1W^M<X6S,%^V>'RJ<G6$<WRP>8)M1G&D;U.P<'^V=CR#<WRE,%FU97RJ9SR6<G2F
=GRJ<G5M5X2S;7NF4X6U,&.D97RF7#R49W&M:6EM5X""B9WFO:SR""<G>M:3R#<X*E:8*4>(FM:3R0>82M
;7ZF,&.I972P>SR""<'FH<GVF<H1M47&S:WFO4#R.98*H;7Z3,%VB=G>J<F9M27ZD<W2J<G=+5X2Z<'5[
)%.P<7VF<H1M2FJ;;(6O786B<CV.-$)M-T5M*EAQ-%:'2E:'2CQG3$!Q-$!Q-%:',#:)-$""'-E%V14!M
*EAQ-$!Q-$!Q-#QN-3QQ,$!M-#QR-$!M-4!Q,$!M-#QR,$-M-#QY,$%Q,$%Q,$)Q,$%+5X2Z<'5[)%2F
:G&V<(1M2FJ;;(6O786B<CV.-$)M.$9M*EAQ-%:'2E:'2CQG3$!Q-$!Q-%:',#:)-$""'-E%V14!M*EAQ
-$!Q-$!Q-#QN-3QQ,$!M-#QR-$!M-4!Q,$!M-#QR,$-M-#QS,$%Q,$%Q,$)Q,$%+5X2Z<'5[)&.515:'
,/;!H?;[E/G\E?3^ES""):7&W?3QT/#QG3$!Q2E:'2E:',#:)-$!Q-$!Q2E9M*EAQ-$!Q-$!Q-#QG3$!Q
-$!Q-$!Q,$!M-#QQ,$!M-4!Q,$%Q-#QQ,$!M-3QS,$!M/#QR-#QR-#QS-#QR#F.U?7RF/C""05#V$3#TF
D9\FOL@FH)<EP:.8.3B1+3QU-CQG3$!Q2E:'2E:',#:)-$!Q-$!Q2E9M*EAQ-%5W/51V-#QG3$!Q2E:'
2E:',$!M-#QQ,$!M-4!Q,$%Q-#QQ,$!M-3QT,$)M/#QS-#QS-#QS-#QR#F.U?7RF/C""05#V+5#R%2F""1
4V"".;8AN6T5M.$)M*EAQ-%:'2E:'2CQG3$!Q-$!Q-%:',#:)-$""&.DF%.4!M*EAQ-%:'2E:'2CQQ,$!M
-#QQ,$%Q-#QR-$!M-#QQ,$%M-SQS,$)M-D!M-D!M-D!M-1J4>(FM:4IA251N1UAM2%:13W&J5WBV6T5N
65YM.$5M*EAQ-%:'2E:'2CQG3$!Q-$!Q-%:',#:)-$!Q-$1Z2E9M*EAQ-$!Q14*'2CQQ,$!M-#QQ,$%Q
-#QR-$!M-#QQ,$%M-CQT,$AM-4!M-4!M-D!M-1J4>(FM:4IA251N3F!M2%:13W&J5WBP,5*E,$1V,#:)
-$""'2E:'2E9M*EAQ-$!Q-$""'2CQG3$!Q-$!U/5:',#:)-$!Q-%%S2E9M-#QQ,$!M-#QR-$!M-4!Q,$!M
-#QR,$)M-SQS,$%Q,$%Q,$)Q,$%+#FN&>G6O>(.>#E:P=GVB>$IA4'&Z:8)M5X2B=H1M27ZE,&.U?7RF
,%ZB<75M47&S:WFO4#R.98*H;7Z3,%VB=G>J<F9M27:G:7.U,&2F?(1+2'FB<'^H>75[)$!M-$IQ-$IQ
-#YU-SQQ/D!Q/D%V,DAV,%^1,5J1,#QQ,$!M-#RB97%\9G*C/W.D9SR\8'*F.6RC<(6S-FRG971I-45Q
,$)Q-#F^[+;,Z,C+YY'3YY+,Z[G[YY'LZ;3K[:G^YY#!YY'JYY+4YY'KZ,O+ZJ?FYY'LYY+#[,S>YY'0
#E2J97RP:X6F/C!Q,$![-$![-$!O.$-M-$IQ-$IR.3YY.3R05#V$3#QM-#QQ,$!M1G&O<G6S/T%S,D=\
-4MN-3YX,(N=9G5V8'*M>8)S8':B:#AR.4!M-D!Q+8XEO\$GH*PFJ+HHK<LHGI4FJ+LJG,-M)/3\CO7E
K?3ZH_;8I/;PF/G8KOC!A!J%;7&M<W>V:4IA-#QQ/D!Q/D%W,D-V,$![-$![-4EO/$%M4V!N3F!M,$!M
-#QQ,#R\8'*F.6RC<(6S-FRG971I-45Q,$)Q-#F^YY'AYY',YY+*)%>P)'VZ)(>B?1J%;7&M<W>V:4IA
-#QQ/D!Q/D%W,D-V,$![-$![-4EO/$%M4V!N1UAM,$!M-#QQ,#R\8'*F.6RC<(6S-FRG971I-45Q,$)Q
-#F^ZJWFZ:#H)%>P)'VZ)(>B?1I+7U:P<H2T81JG<WZU<G&N:4IA<G&N:3ZC<8!+/4=K2$IB#G:P<H2O
97VF/C""O97VF)#AS+8]O9GVQ#DEX+E1[*T5+:G^O>'ZB<75[)/3YL?;8J?G@K3ZC<8!+/4=K2$IH.E=+
#FN(=G&Q;'FD=VU+:GFM:7ZB<75[)%ZP)%6Y>'6O;7^O#DEX+E1[)1JG;7RF<G&N:4IA=X""B9S!A)'6T
,H""O:QIZ.SJ%/C=V#G:J<'6O97VF/C""E:#ZG,G:G,GJQ:QIZ.SJ%/C=W2QI+";

        [DataRow(DATA, BasicTest.DATA)]
        [DataRow("91", "a")]
        [DataRow("97)", "ab")]
        [DataRow("97*D", "abc")]
        [DataRow("97*D:!", "abcd")]
        [DataRow("97*D:'5", "abcde")]
        [DataRow("97*D:'6G", "abcdef")]
        [DataTestMethod]
        public void Encode(string expect, string utf8in)
        {
            expect += Environment.NewLine;
            var bytes = Encoding.UTF8.GetBytes(utf8in.Replace("\r\n", "\n"));
            var writer = new StringWriter();
            UUEncoder.Encode(bytes, writer);
            Assert.That.AreMultiLineStringEquals(expect, writer.ToString());
        }

        [DataRow(DATA, BasicTest.DATA)]
        [DataRow("91", "a")]
        [DataRow("97)", "ab")]
        [DataRow("97*D", "abc")]
        [DataRow("97*D:!", "abcd")]
        [DataRow("97*D:'5", "abcde")]
        [DataRow("97*D:'6G", "abcdef")]
        [DataTestMethod]
        public void Decode(string encoded, string utf8in)
        {
            var d = new UUDecoder();
            foreach (var line in encoded.Split('\r', '\n'))
            {
                d.ReadLine(line);
            }
            var data = d.ToArray();
            var expect = Encoding.UTF8.GetBytes(utf8in.Replace("\r\n", "\n"));

            CollectionAssert.AreEqual(expect, data);
        }
    }
}