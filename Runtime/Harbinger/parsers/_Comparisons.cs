﻿namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseComparison(in BoaReader reader, in Executor caller, out ExpressionExecutor expression)
        {
            expression = null;
            if (TryParseAddSub(reader, caller, out var addsub1))
            {
                if (reader.TryReadChar_match_out(out char op_char, true, "<>="))
                {
                    OperatorsM code = op_char switch
                    {
                        '<' => OperatorsM.lt,
                        '>' => OperatorsM.gt,
                        '=' => OperatorsM.eq,
                        _ => 0,
                    };

                    if (reader.TryReadChar_match('=', skippables: null) && code != 0)
                        code |= OperatorsM.eq;

                    if (TryParseAddSub(reader, caller, out var addsub2))
                    {
                        ContractExecutor exe = new(this, caller, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(code);
                        exe.args.Add(addsub1);
                        exe.args.Add(addsub2);
                        expression = exe;
                        return true;
                    }
                    else
                    {
                        reader.error ??= $"expected expression after '{op_char}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = addsub1;
                    return true;
                }
            }
            return false;
        }
    }
}