package main
import (
    "fmt"
    "github.com/fgrzl/azpos/go/azpos"
)
func main() {
    fmt.Println("Go results:")
    mid, _ := azpos.Midpoint("", "a")
    fmt.Println("Midpoint(\"\", \"a\"):", mid)
    mid2, _ := azpos.Midpoint("a", "c")
    fmt.Println("Midpoint(\"a\", \"c\"):", mid2)
    fmt.Println("Validate(\"ab\"):", azpos.Validate("ab"))
    fmt.Println("Validate(\"a\"):", azpos.Validate("a"))
    fmt.Println("Compare(\"a\", \"b\"):", azpos.Compare("a", "b"))
    reb, _ := azpos.Rebalance3("a", "a")
    fmt.Println("Rebalance3(\"a\", \"a\"):", reb)
}
