const fs = require('fs');

const requiredSum = 2020;

(async function main() {
    const lines = inputToLines();
    const numbers = lines.map(Number);

    // Brute ftw
    for (const firstTerm of numbers) {
        for (const secondTerm of numbers) {
            for (const thirdTerm of numbers) {
                if (firstTerm + secondTerm + thirdTerm === requiredSum) {
                    console.log(firstTerm * secondTerm * thirdTerm);
                }
            }
        }
    }
})();

function inputToLines() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n');
}