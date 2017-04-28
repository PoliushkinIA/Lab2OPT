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
            NoError, Program, Var, Begin, End, Dec, PrcdCall
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
            return program();
        }

        bool program()
        {
            if (prog.Substring(0, 8).Equals("PROGRAM "))
            {
                pos = 8;
                if (id())
                {
                    for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
                    if (prog[pos++]==';')
                    {
                        for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
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
                            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
                            if (prog.Substring(pos, 5).Equals("BEGIN"))
                            {
                                pos += 5;
                                if (prog[pos] != ' ' && prog[pos] != '\n')
                                {
                                    parseError = ParseError.Begin;
                                    return false;
                                }
                                for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
                                if (!stmt_list())
                                    return false;
                                for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
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

        private bool stmt_list()
        {
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
            if (!stmt())
                return false;
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
            if (prog[pos] == ';')
            {
                pos++;
                stmt_list();
            }
            return true;
        }

        private bool stmt()
        {
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
            return _if() || repeat() || read() || write() || prcd_call() || assign();
        }

        private bool assign()
        {
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
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
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
            return false;
        }

        private bool write()
        {
            return false;
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
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
            if (!dec())
                return false;
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
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
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
            if (prog[pos++] != ':')
                return false;
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
            if (!prog.Substring(pos, 7).Equals("INTEGER") && !prog.Substring(pos, 7).Equals("BOOLEAN"))
                return false;
            pos += 7;
            return true;
        }

        private bool id_list()
        {
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
            if (!id())
                return false;
            for (; prog[pos] == ' ' || prog[pos] == '\n'; pos++) ;
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
