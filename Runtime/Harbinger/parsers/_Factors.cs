﻿using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseFactor(in BoaReader reader, in Executor caller, out ExpressionExecutor factor)
        {
            factor = null;

            if (reader.error == null)
                if (reader.TryReadChar_match('('))
                    if (!TryParseExpression(reader, caller, false, out factor))
                    {
                        reader.error ??= "expected expression inside factor parenthesis";
                        return false;
                    }
                    else if (!reader.TryReadChar_match(')'))
                    {
                        reader.error ??= $"expected closing parenthesis ')' after factor {factor.ToLog}";
                        --reader.read_i;
                        return false;
                    }
                    else
                        return true;

            if (reader.error == null)
                if (TryParseString(reader, out string str))
                {
                    factor = new LiteralExecutor(this, caller, literal: str);
                    return true;
                }

            if (reader.error == null)
                if (reader.TryReadArgument(out string arg, false))
                    if (caller._functions.TryGet(arg, out var func))
                    {
                        factor = new ContractExecutor(this, caller, func, reader);
                        if (factor.error != null)
                        {
                            reader.error = factor.error;
                            return false;
                        }
                        return true;
                    }
                    else if (global_contracts.TryGetValue(arg, out var contract))
                    {
                        factor = new ContractExecutor(this, caller, contract, reader);
                        if (factor.error != null)
                        {
                            reader.error = factor.error;
                            return false;
                        }
                        return true;
                    }
                    else if (caller._variables.TryGet(arg, out var variable))
                    {
                        factor = new VariableExecutor(this, caller, variable);
                        return true;
                    }
                    else
                        switch (arg.ToLower())
                        {
                            case "true":
                                factor = new LiteralExecutor(this, caller, literal: true);
                                return true;

                            case "false":
                                factor = new LiteralExecutor(this, caller, literal: false);
                                return true;

                            default:
                                if (int.TryParse(arg, out int _int))
                                    factor = new LiteralExecutor(this, caller, literal: _int);
                                else if (Util.TryParseFloat(arg, out float _float))
                                    factor = new LiteralExecutor(this, caller, literal: _float);
                                else
                                {
                                    reader.error ??= $"unrecognized object : '{arg}'";
                                    return false;
                                }
                                return true;
                        }

            return false;
        }
    }
}