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
            NoError, Program, Var, Begin, End, Dec, PrcdCall, Read, Write, UEOF, Assign, Expr, If, Repeat, Cond, Procedure
        }

        string prog;
        int pos = 0;
        int linePos = -1;
        public ParseError parseError;

        public Parser(string program)
        {
            prog = program;
        }
        
        public int GetLine()
        {
            int line = 0;
            for (int i = 0; i <= pos; i++, linePos++)
            {
                if (prog[i] == '\n')
                {
                    line += 1;
                    linePos = 0;
                }
            }
            return line;
        }

        public int GetPlace()
        {
            return pos - linePos + 1;
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
            if (prog.Substring(pos, 8).Equals("PROGRAM "))
            {
                pos += 8;
                if (id())
                {
                    Skip();
                    if (prog[pos++] == ';')
                    {
                        Skip();
                        if (!prcd_list())
                            return false;
                        Skip();
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
                                    if (parseError == ParseError.NoError)
                                    {
                                        parseError = ParseError.End; 
                                    }
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
            parseError = ParseError.NoError;
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
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Assign; 
                }
                return false;
            }
            Skip();
            if (!(prog[pos++] == ':' && prog[pos++] == '='))
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Assign; 
                }
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
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Expr; 
                }
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
                Skip();
                if (prog[pos++] != ')')
                    return false;
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
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.PrcdCall; 
                }
                return false;
            }
            if (prog[pos++] != '(')
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.PrcdCall; 
                }
                return false;
            }
            if (!id_list())
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.PrcdCall; 
                }
                return false;
            }
            Skip();
            if (prog[pos++] != ')')
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.PrcdCall; 
                }
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
                    if (parseError == ParseError.NoError)
                    {
                        parseError = ParseError.Read; 
                    }
                    return false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Read; 
                }
                throw;
            }
            pos += 4;
            if (prog[pos++] != '(')
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Read; 
                }
                return false;
            }
            if (!id_list())
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Read; 
                }
                return false;
            }
            Skip();
            if (prog[pos++] != ')')
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Read; 
                }
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
                    if (parseError == ParseError.NoError)
                    {
                        parseError = ParseError.Write; 
                    }
                    return false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Write; 
                }
                return false;
            }
            pos += 5;
            if (prog[pos++] != '(')
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Write; 
                }
                return false;
            }
            if (!id_list())
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Write; 
                }
                return false;
            }
            Skip();
            if (prog[pos++] != ')')
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Write; 
                }
                return false;
            }
            return true;
        }

        private bool repeat()
        {
            try
            {
                int curPos = pos;
                if (!prog.Substring(pos, 6).Equals("REPEAT"))
                {
                    pos = curPos;
                    if (parseError == ParseError.NoError)
                    {
                        parseError = ParseError.Repeat; 
                    }
                    return false;
                }
                pos += 6;
                Skip();
                if (!stmt_list())
                {
                    pos = curPos;
                    if (parseError == ParseError.NoError)
                    {
                        parseError = ParseError.Repeat; 
                    }
                    return false;
                }
                Skip();
                if (!prog.Substring(pos, 5).Equals("UNTIL"))
                {
                    pos = curPos;
                    if (parseError == ParseError.NoError)
                    {
                        parseError = ParseError.Repeat; 
                    }
                    return false;
                }
                pos += 5;
                Skip();
                if (!cond())
                {
                    pos = curPos;
                    return false;
                }
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.Repeat; 
                }
                return false;
            }
        }

        private bool _if()
        {
            int curPos = pos;
            Skip();
            if (!prog.Substring(pos, 2).Equals("IF"))
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.If; 
                }
                return false;
            }
            pos += 2;
            Skip();
            if (!cond())
            {
                pos = curPos;
                return false;
            }
            Skip();
            if (!prog.Substring(pos, 4).Equals("THEN"))
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.If; 
                }
                return false;
            }
            pos += 4;
            Skip();
            if (!body())
            {
                pos = curPos;
                if (parseError == ParseError.NoError)
                {
                    parseError = ParseError.If; 
                }
                return false;
            }
            Skip();
            if (prog.Substring(pos, 4).Equals("ELSE"))
            {
                pos += 4;
                if (!body())
                {
                    pos = curPos;
                    if (parseError == ParseError.NoError)
                    {
                        parseError = ParseError.If; 
                    }
                    return false;
                }
            }
            Skip();
            return true;
        }

        private bool body()
        {
            try
            {
                if (prog.Substring(pos, 5).Equals("BEGIN"))
                {
                    pos += 5;
                    Skip();
                    if (!stmt_list())
                        return false;
                    Skip();
                    if (!prog.Substring(pos, 3).Equals("END"))
                        return false;
                    pos += 3;
                    Skip();
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            return stmt();
        }

        private bool cond()
        {
            try
            {
                int curPos = pos;
                if (expr())
                {
                    Skip();
                    if (cmp_op())
                    {
                        Skip();
                        if (!expr())
                        {
                            pos = curPos;
                            parseError = ParseError.Cond;
                            return false;
                        }
                        Skip();
                        return true;
                    }
                }
                pos = curPos;
                if (prog[pos]=='(')
                {
                    pos++;
                    if (!cond())
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    if (prog[pos++] != ')')
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    if (!bool_op())
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    if (prog[pos++] != '(')
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    if (!cond())
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    if (prog[pos++] != ')')
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    return true;
                }
                if (prog.Substring(pos, 3).Equals("NOT"))
                {
                    pos += 3;
                    Skip();
                    if (prog[pos++] != '(')
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    if (!cond())
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    if (prog[pos++] != ')')
                    {
                        pos = curPos;
                        parseError = ParseError.Cond;
                        return false;
                    }
                    Skip();
                    return true;
                }
                if (!id())
                {
                    pos = curPos;
                    parseError = ParseError.Cond;
                    return false;
                }
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                parseError = ParseError.Cond;
                return false;
            }
        }

        private bool bool_op()
        {
            if (prog.Substring(pos, 2).Equals("OR"))
            {
                pos += 2;
                return true;
            }
            if (prog.Substring(pos, 3).Equals("AND"))
            {
                pos += 3;
                return true;
            }
            return false;
        }

        private bool cmp_op()
        {
            if (prog.Substring(pos, 2).Equals("<=") || prog.Substring(pos, 2).Equals(">=") || prog.Substring(pos, 2).Equals("<>"))
            {
                pos += 2;
                return true;
            }
            if ("=><".Contains(prog[pos]))
            {
                pos++;
                return true;
            }
            return false;
        }

        private bool dec_list()
        {
            Skip();
            if (!dec())
                return false;
            Skip();
            if (prog[pos]==';')
            {
                pos++;
                dec_list();
            }
            return true;
        }

        private bool dec()
        {
            int curPos = pos;
            if (!id_list())
            {
                parseError = ParseError.Dec;
                pos = curPos;
                return false;
            }
            Skip();
            if (prog[pos++] != ':')
            {
                parseError = ParseError.Dec;
                pos = curPos;
                return false;
            }
            Skip();
            if (!prog.Substring(pos, 7).Equals("INTEGER") && !prog.Substring(pos, 7).Equals("BOOLEAN"))
            {
                parseError = ParseError.Dec;
                pos = curPos;
                return false;
            }
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
            int curPos = pos;
            Skip();
            if (!procedure())
            {
                pos = curPos;
                return true;
            }
            Skip();
            if (prog[pos++] != ';')
            {
                parseError = ParseError.Procedure;
                return false;
            }
            return prcd_list();
        }

        private bool procedure()
        {
            if (prog.Substring(pos, 10).Equals("PROCEDURE "))
            {
                pos += 10;
                if (id())
                {
                    Skip();
                    if (prog[pos++] != '(')
                        return false;
                    Skip();
                    if (!dec_list())
                        return false;
                    Skip();
                    if (prog[pos++] != ')')
                        return false;
                    Skip();
                    if (prog[pos++] == ';')
                    {
                        Skip();
                        if (!prcd_list())
                            return false;
                        Skip();
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
                                if (prog.Substring(pos, 3).Equals("END"))
                                {
                                    pos += 3;
                                    return true;
                                }
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
                        parseError = ParseError.Procedure;
                        return false;
                    }
                }
                else
                {
                    parseError = ParseError.Procedure;
                    return false;
                }
            }
            else
            {
                parseError = ParseError.Procedure;
                return false;
            }
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
