using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Converter
{
    public class NumToString : MonoBehaviour
    {
        /*********************************************************************
        * @function    : NumIntToWords(String NumIn)
        * @purpose     : Converts Unsigned Integers to Words
        *                Using the SLST Method (C) Mohsen Alyafei 2019.
        *                Does not check for Non-Numerics or +/- signs
        * @version     : 2.11
        * @author      : Mohsen Alyafei
        * @Licence     : MIT
        * @date        : 03 April 2022
        * @in_param    : {NumIn} (Required): Number in string format
        * @returns     : {string}: The number in English Words US Grammar
        **********************************************************************/
        public static string NumIntToWords(string NumIn)
        {
            if (NumIn.TrimStart('0') == "") return "Zero";                   // If empty or zero return Zero

            string[] T10s = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" },
                     T20s = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" },
                   Scales = { "", "Thousand", "Million", "Billion", "Trillion" };// increase scale here (unlimited)
            NumIn = new string('0', NumIn.Length * 2 % 3) + NumIn;        // Make it a String Triplet
            string numToWords = "", wordTriplet, Triplet;

            for (int digits = NumIn.Length; digits > 0; digits -= 3)
            {   // Single Loop
                Triplet = NumIn.Substring(NumIn.Length - digits, 3);      // Get next Triplet
                if (Triplet != "000")
                {                                   // Convert Only if not empty
                    wordTriplet = "";
                    int ScalePos = digits / 3 - 1,                           // Scale name position
                            hund = int.Parse("" + Triplet[0]),
                            tens = int.Parse(Triplet.Substring(1, 2)),
                            ones = int.Parse("" + Triplet[2]);
                    wordTriplet = (hund > 0 ? T10s[hund] + " Hundred" : "") +
                                    (tens > 0 && hund > 0 ? " " : "") +
                                    (tens < 20 ? T10s[tens] : T20s[int.Parse("" + Triplet[1])] + (ones > 0 ? "-" + T10s[ones] : "")) +
                                    (ScalePos > 0 ? " " : "") + Scales[ScalePos];    // Add Scale Name to Triplet Word
                    numToWords += (numToWords != "" ? " " : "") + wordTriplet;    // Concat Next Triplet Word
                }
            }                   // Loop for the next Triplet
            return numToWords;  // Return full Number in Words
        }
    }
}
