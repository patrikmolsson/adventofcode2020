
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
    const hasOnFirst = line.string.charAt(line.lowerRange - 1) === line.characterToLookFor;
    const hasOnSecond = line.string.charAt(line.higherRange - 1) === line.characterToLookFor;

    return !!((hasOnFirst ? 1 : 0) ^ (hasOnSecond ? 1 : 0));
}

const lines = input.split('\n');

let totalCount = 0;
for (const line of lines) {
    if (processLine(parseLine(line))) {
        totalCount += 1;
    }
}
console.log(totalCount);