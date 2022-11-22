# WFC Sudoku Solver

A simple sudoku solver using the Wave Function Collapse algorithm.

See https://www.youtube.com/watch?v=rI_y2GAlQFM for a nice video on how it works.

## Demo

![Demo](https://github.com/EttienneS/WFC-Sudoku-Solver/raw/master/demo.gif)

## Stats

When not in demo mode it generally solves the puzzles in:

Simple: ~1ms
Easy:  ~40ms
Medium: ~55ms
Hard: ~300ms
Hardest: ~25000ms

Because of the random nature of WFC solve times can vary greatly, I have seen hardest solve in 4 000ms and other times 35 000 ms. But it will always find a solution eventually.

This can also generate puzzles, this can be done in ~40ms

## Improvement

The best improvement that can be made would be to make the 'rollback' feature smarter, currently it always goes back to step 0 but if we can make it rather choose a 'stable' snapshot in the past to cut down on regeneration it would greatly speed up the process.