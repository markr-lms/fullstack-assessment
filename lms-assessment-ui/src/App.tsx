import Button from "@mui/material/Button";
import Container from "@mui/material/Container";
import Divider from "@mui/material/Divider";
import Stack from "@mui/material/Stack";

function App() {
  const handleAdd = () => console.log("todo");

  return (
    <Container maxWidth="md">
      <header>
        <h1>LMS.Assessment</h1>
      </header>
      <Stack component="main" spacing={2}>
        <section id="add-firm">
          <Button variant="contained" onClick={handleAdd}>
            Add Law Firm
          </Button>
        </section>
        <Divider />
        <section id="list-firms">
          <h2>List of Law Firms</h2>
        </section>
      </Stack>
    </Container>
  );
}

export default App;
