package main

import (
	"fmt"
	"io/ioutil"
	"log"
	"os"
	"path/filepath"
)

func main() {
	scan("../")
}

func scan(path string) {
	path, _ = filepath.Abs(path)

	fmt.Println("================================================================")
	fmt.Printf("START: %s\n", path)

	entries, err := ioutil.ReadDir(path)
	if err != nil {
		log.Fatal(err)
	}

	if len(entries) == 0 {
		fmt.Println("  <EMPTY>")
		fmt.Printf("END: %s\n", path)
		return
	}

	dirs := []os.FileInfo{}
	files := []os.FileInfo{}

	for _, f := range entries {
		if f.IsDir() {
			dirs = append(dirs, f)
		} else {
			files = append(files, f)
		}
	}

	for _, d := range dirs {
		fmt.Printf("  %s\t%s\t%s\t%s\n", d.ModTime(), d.Mode(), "<DIR>", d.Name())
	}

	for _, f := range files {
		fmt.Printf("  %s\t%s\t%d\t%s\n", f.ModTime(), f.Mode(), f.Size(), f.Name())
	}

	fmt.Printf("END: %s\n", path)

	for _, d := range dirs {
		scan(filepath.Join(path, d.Name()))
	}
}
