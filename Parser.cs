using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPTLab2
{
    class Parser
    {
        public enum ParseError
        {
            NoError, Program, Var, Begin, End, Dec, PrcdCall, Read, Write, UEOF, Assign, Expr
        }

        string prog;
        int pos;
        public ParseError parseError;

        public Parser(string program)
        {
            prog = program;
        }

        public bool Parse()
        {
            try
            {
                return program();
            }
            catch (ArgumentOutOfRangeException)
            {
                parseError = ParseError.UEOF;
                return false;
            }
        }

        bool program()
        {
            if (prog.Substring(0, 8).Equals("PROGRAM "))
            {
                pos = 8;
                if (id())
                {
                    Skip();
                    if (prog[pos++] == ';')
                    {
                        Skip();
                        if (!prcd_list())
                            return false;
                        if (prog.Substring(pos, 3).Equals("VAR"))
                        {
                            pos += 3;
                            if (!dec_list())
                            {
                                parseError = ParseError.Var;
                                return false;
                            }
                            Skip();
                            if (prog.Substring(pos, 5).Equals("BEGIN"))
                            {
                                pos += 5;
                                if (prog[pos] != ' ' && prog[pos] != '\n')
                                {
                                    parseError = ParseError.Begin;
                                    return false;
                                }
                                Skip();
                                if (!stmt_list())
                                    return false;
                                Skip();
                                if (prog.Substring(pos, 4).Equals("END."))
                                    return true;
                                else
                                {
                                    parseError = ParseError.End;
                                    return false;
                                }
                            }
                            else
                            {
                                parseError = ParseError.Begin;
                                return false;
                            }
                        }
                        else
                        {
                            parseError = ParseError.Var;
                            return false;
                        }
                    }
                    else
                    {
                        parseError = ParseError.Program;
                        return false;
                    }
                }
                else
                {
                    parseError = ParseError.Program;
                    return false;
                }
            }
            else
            {
                parseError = ParseError.Program;
                return false;
            }
        }

        // Skips the whitespaces
        private void Skip()
        {
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
        }

        private bool stmt_list()
        {
            Skip();
            if (!stmt())
                return false;
            Skip();
            if (prog[pos] == ';')
            {
                pos++;
                stmt_list();
            }
            return true;
        }

        private bool stmt()
        {
            Skip();
            return _if() || repeat() || read() || write() || prcd_call() || assign();
        }

        private bool assign()
        {
            int curPos = pos;
            if (!id())
            {
                pos = curPos;
                parseError = ParseError.Assign;
                return false;
            }
            Skip();
            if (!(prog[pos++] == ':' && prog[pos++] == '='))
            {
                pos = curPos;
                parseError = ParseError.Assign;
                return false;
            }
            Skip();
            if (!expr())
            {
                pos = curPos;
                return false;
            }
            return true;
        }

        private bool expr()
        {
            if (!term())
            {
                parseError = ParseError.Expr;
                return false;
            }
            Skip();
            if (prog[pos] == '+' || prog[pos] == '-')
            {
                pos++;
                Skip();
                return expr();
            }
            return true;
        }

        private bool term()
        {
            if (!factor())
                return false;
            Skip();
            if (prog[pos] == '*' || prog[pos] == '/')
            {
                pos++;
                Skip();
                return term();
            }
            return true;
        }

        private bool factor()
        {
            if (prog[pos] == '-' || prog[pos] == '+')
                pos++;
            return rfactor();
        }

        private bool rfactor()
        {
            if (prog[pos] == '(')
            {
                pos += 1;
                if (!expr())
                    return false;
                pos += 1;
                return true;
            }
            if (digit(prog[pos]))
                return integer();
            if (letter(prog[pos]))
                return id();
            return false;
        }

        private bool prcd_call()
        {
            int curPos = pos;
            if (!id())
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            if (prog[pos++] != '(')
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            if (!id_list())
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            Skip();
            if (prog[pos++] != ')')
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            return true;
        }

        private bool read()
        {
            int curPos = pos;
            try
            {
                if (!prog.Substring(pos, 4).Equals("READ"))
                {
                    pos = curPos;
                    parseError = ParseError.Read;
                    return false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                parseError = ParseError.Read;
                throw;
            }
            pos += 4;
            if (prog[pos++] != '(')
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            if (!id_list())
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            Skip();
            if (prog[pos++] != ')')
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            return true;
        }

        private bool write()
        {
            int curPos = pos;
            try
            {
                if (!prog.Substring(pos, 5).Equals("WRITE"))
                {
                    pos = curPos;
                    parseError = ParseError.Write;
                    return false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                parseError = ParseError.Write;
                return false;
            }
            pos += 5;
            if (prog[pos++] != '(')
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            if (!id_list())
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            Skip();
            if (prog[pos++] != ')')
            {
                pos = curPos;
                parseError = ParseError.PrcdCall;
                return false;
            }
            return true;
        }

        private bool repeat()
        {
            return false;
        }

        private bool _if()
        {
            return false;
        }

        private bool dec_list()
        {
            Skip();
            if (!dec())
                return false;
            Skip();
            if (prog[pos++]!=';')
            {
                parseError = ParseError.Dec;
                return false;
            }
            dec_list();
            return true;
        }

        private bool dec()
        {
            if (!id_list())
                return false;
            Skip();
            if (prog[pos++] != ':')
                return false;
            Skip();
            if (!prog.Substring(pos, 7).Equals("INTEGER") && !prog.Substring(pos, 7).Equals("BOOLEAN"))
                return false;
            pos += 7;
            return true;
        }

        private bool id_list()
        {
            Skip();
            if (!id())
                return false;
            Skip();
            if (prog[pos] == ',')
            {
                pos++;
                id_list();
            }
            return true;
        }

        private bool prcd_list()
        {
            return true;
        }

        private bool id()
        {
            int curPos = pos;
            if (!letter(prog[curPos++]))
                return false;
            for (; letter(prog[curPos]) || digit(prog[curPos]); curPos++) ;
            if (prog.Substring(pos, curPos - pos).Equals("IF") ||
                prog.Substring(pos, curPos - pos).Equals("THEN") ||
                prog.Substring(pos, curPos - pos).Equals("ELSE") ||
                prog.Substring(pos, curPos - pos).Equals("REPEAT") ||
                prog.Substring(pos, curPos - pos).Equals("UNTIL") ||
                prog.Substring(pos, curPos - pos).Equals("BEGIN") ||
                prog.Substring(pos, curPos - pos).Equals("END") ||
                prog.Substring(pos, curPos - pos).Equals("VAR") ||
                prog.Substring(pos, curPos - pos).Equals("PROGRAM") ||
                prog.Substring(pos, curPos - pos).Equals("PROCEDURE") ||
                prog.Substring(pos, curPos - pos).Equals("READ") ||
                prog.Substring(pos, curPos - pos).Equals("WRITE") ||
                prog.Substring(pos, curPos - pos).Equals("INTEGER") ||
                prog.Substring(pos, curPos - pos).Equals("BOOLEAN") ||
                prog.Substring(pos, curPos - pos).Equals("AND") ||
                prog.Substring(pos, curPos - pos).Equals("OR") ||
                prog.Substring(pos, curPos - pos).Equals("NOT"))
                return false;
            pos = curPos;
            return true;
        }

        private bool integer()
        {
            int curPos = pos;
            for (; digit(prog[curPos]); curPos++) ;
            pos = curPos;
            return true;
        }

        private bool digit(char v)
        {
            return v >= '0' && v <= '9';
        }

        private bool letter(char v)
        {
            return v >= 'A' && v <= 'Z';
        }
    }
}
