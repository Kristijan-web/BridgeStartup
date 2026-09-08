import { readdir, readFile, mkdir, writeFile } from 'node:fs/promises';
import path from 'node:path';
import { spawnSync } from 'node:child_process';
import ts from 'typescript';

const output = path.resolve('tmp/frontend-tests');
async function compile(directory) {
  for (const entry of await readdir(directory, { withFileTypes: true })) {
    const file = path.join(directory, entry.name);
    if (entry.isDirectory()) await compile(file);
    else if (file.endsWith('.ts')) {
      const target = path.join(output, file.replace(/\.ts$/, '.mjs'));
      const source = await readFile(file, 'utf8');
      const result = ts.transpileModule(source, { compilerOptions: {
        target: ts.ScriptTarget.ES2022, module: ts.ModuleKind.ES2022, experimentalDecorators: true
      } }).outputText.replace(/(from\s+['"])(\.[^'"]+)(['"])/g, '$1$2.mjs$3');
      await mkdir(path.dirname(target), { recursive: true });
      await writeFile(target, result);
    }
  }
}
await compile('src/app');
const result = spawnSync(process.execPath, ['--test', 'tests/frontend.test.mjs'], { stdio: 'inherit' });
process.exitCode = result.status ?? 1;
