
const input = await Deno.readTextFile(`${Deno.cwd()}/input.txt`);

function parseLine(line: string) {
    const lineSplit = line.split(": ");
    const prefix = lineSplit[0];
    const string = lineSplit[1];

    const characterToLookFor = prefix.charAt(prefix.length - 1);
    const rangeString = prefix.slice(0, prefix.length - 2);

    const rangeStringSplit = rangeString.split('-');
    const lowerRange = rangeStringSplit[0];
    const higherRange = rangeStringSplit[1];

    return {
        string,
        characterToLookFor,
        lowerRange: Number(lowerRange),
        higherRange: Number(higherRange),
    };
}

function processLine(line: ReturnType<typeof parseLine>) {
    console.log(line);
    const count = line.string.split('').filter(f => f === line.characterToLookFor).length;

    return count >= line.lowerRange && count <= line.higherRange;
}

const lines = input.split('\n');

let totalCount = 0;
for (const line of lines) {
    if (processLine(parseLine(line))) {
        totalCount += 1;
    }
}
console.log(totalCount);