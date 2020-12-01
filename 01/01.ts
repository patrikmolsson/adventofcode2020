const fs = require('fs');

const mapOfRequiredTerms: Record<number, boolean> = {};
const requiredSum = 2020;

(async function main() {
    const lines = inputToLines();
    const numbers = lines.map(Number);

    for (const number of numbers) {
        if (mapOfRequiredTerms[number]) {
            console.log(number * (requiredSum - number));
        }

        mapOfRequiredTerms[requiredSum - number] = true;
    }
})();

function inputToLines() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');



    return lines.split('\r\n');
}