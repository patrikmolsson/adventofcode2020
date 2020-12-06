const fs = require('fs');


(function main() {
    const groups = inputToGroupAnswers();

    let sum = 0;
    for (const group of groups) {
        const replies = group.split(' ').map(reply => reply.split(''));

        const allYes = new Set([...intersect(...replies)]);

        sum += allYes.size;
    }

    console.log(sum);
})();

function intersect(...params: string[][]): string[] {
    return params.reduce((a, b) => a.filter(c => b.includes(c)))
}

function inputToGroupAnswers(): string[] {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n\r\n').map(p => p.replace(/\r\n/g, ' '), '');
}