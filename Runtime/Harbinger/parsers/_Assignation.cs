﻿namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAssignation(in BoaReader reader, in Executor caller, out ExpressionExecutor assignation)
        {
            assignation = null;
            int read_old = reader.read_i;

            if (reader.TryReadArgument(out string varname, as_function_argument: false))
                if (caller._variables.TryGet(varname, out var variable))
                    if (reader.TryReadString_matches_out(out string op_name, skippables: BoaReader._empties_, stoppers: " \n\r{}(),;'\"", "=", "+=", "-=", "*=", "/="))
                    {
                        OperatorsM code = op_name switch
                        {
                            "=" => OperatorsM.eq,
                            "+=" => OperatorsM.add,
                            "-=" => OperatorsM.sub,
                            "*=" => OperatorsM.mul,
                            "/=" => OperatorsM.div,
                            _ => OperatorsM.unknown,
                        };

                        code |= OperatorsM.assign;

                        if (TryParseExpression(reader, caller, false, out var expr))
                        {
                            ContractExecutor exe = new(this, caller, cmd_assign_, reader, parse_arguments: false);
                            exe.args.Add(code);
                            exe.args.Add(variable);
                            exe.args.Add(expr);
                            assignation = exe;
                            return true;
                        }
                        else
                        {
                            reader.error ??= $"expected expression after '{op_name}' operator";
                            return false;
                        }
                    }

            reader.read_i = read_old;
            return false;
        }
    }
}