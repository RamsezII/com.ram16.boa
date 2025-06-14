﻿namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseOr(in BoaReader reader, in Executor caller, out ExpressionExecutor expression)
        {
            expression = null;
            if (TryParseAnd(reader, caller, out var or1))
                if (reader.TryReadString_match_out(out string op_name, "or"))
                {
                    if (TryParseAnd(reader, caller, out var or2))
                    {
                        ContractExecutor exe = new(this, caller, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(OperatorsM.or);
                        exe.args.Add(or1);
                        exe.args.Add(or2);
                        expression = exe;
                        return true;
                    }
                    else
                    {
                        reader.error ??= $"expected expression after '{op_name}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = or1;
                    return true;
                }
            return false;
        }
    }
}