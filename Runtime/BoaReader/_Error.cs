namespace _BOA_
{
    partial class BoaReader
    {
        public void LocalizeError(in string script_path, in string[] lines)
        {
            int char_count = 0;
            int eol_lenth = Util.is_windows ? 2 : 1;

            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
                if (i == lines.Length - 1 || !string.IsNullOrWhiteSpace(line) && char_count + line.Length >= read_i)
                {
                    int char_i = read_i - char_count + 6;
                    string spaces = new(' ', char_i);
                    long_error = $"at {script_path}:{i}\n({nameof(last_arg)}: '{last_arg}', {i}, {char_i})\n {i + ".",-4} {line}\n{spaces}|\n{spaces}└──> {error}";
                    return;
                }
                char_count += eol_lenth + line.Length;
            }
            long_error = $"\n{error}";
        }
    }
}